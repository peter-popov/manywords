module XlsReader

open System
open System.IO
open Microsoft.Office.Interop.Excel

open Word


type XlsColumns = 
    | Word
    | Translation
    | PartOfSpeach
    | Gender
    | Forms


//
//Read cell from Excell
let getCell (cells : Range ) (c : string ) ( r: int) =
    let id : string = c.ToString() + r.ToString()
    let value = cells.Range(id).Value2
    if value <> null then value.ToString() else "" 

      
//
//Read first line
let readLine (cells : Range ) n =
    let range = cells.Range("A"+n.ToString(),"G"+n.ToString()).Value2 :?> obj[,]
    let columns = seq { for i in 'A'..'G' do yield i.ToString() }
    let values = seq { for v in range do yield v.ToString() }
    Seq.zip columns values
    
//Match columns to word fields
let matchColumns language header =        
    [ for (column, title) in header do
        match title with
        | Match "(gender)" _ -> yield ( XlsColumns.Gender, column)
        | Match "(plural|forms)" _ -> yield ( XlsColumns.Forms, column)     
        | Match "(part)(.*)(speech)" _ -> yield ( XlsColumns.PartOfSpeach, column)
        | Match language _ -> yield ( XlsColumns.Word, column)
        | Match "english" _ -> yield ( XlsColumns.Translation, column)                                           
        | _ -> ignore None 
    ] |> Map.ofSeq
    
    
let readWord ( cells: Range ) columns row lng =
    let readColumn x = if Map.containsKey x columns then 
                        getCell cells ( Map.find x columns) row else
                        ""
    { Word.Language = lng
      Word.Spelling = readColumn XlsColumns.Word;
      Word.Translation = readColumn XlsColumns.Translation;
      Word.Part = matchPartOfSpeach (readColumn XlsColumns.PartOfSpeach);
      Word.Gender = matchGender (readColumn XlsColumns.Gender);
      Word.Forms = readColumn XlsColumns.Forms }     
    
//
//Extract list of words    
let extractDictionary (cells : Range ) columns lng =
    let continueLoop = ref true
    let i = ref 1    
    seq {
    while !continueLoop do 
        i := !i + 1
        let word = readWord cells columns !i lng
        if word.Spelling <> "" then
            yield word
        else
            continueLoop := false
    }

let app = new ApplicationClass(Visible = false)
    
    
//
//Read words from file    
let readFile file =
    let book = app.Workbooks.Open file
    let sheet = book.Worksheets.[1] :?> _Worksheet 
    let b1 = getCell sheet.Cells "B" 1
    let language = 
        if b1.Trim().ToLower().Equals("english") then 
            getCell sheet.Cells "C" 1
        else
            b1 
    let header = readLine sheet.Cells 1
    let r = extractDictionary sheet.Cells (matchColumns language header) language   
    printf "%s dine" file
    r 