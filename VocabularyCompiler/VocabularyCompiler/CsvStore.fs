module CsvStore

open System
open System.IO

open Word

let separator = [|'$'; '\t'|]

let fromString (s:string) =
    let l = s.Split(separator)
    if l.Length >= 5 then
        Some { 
        Word.Language = l.[0];
        Word.Spelling = l.[1];
        Word.Translation = l.[2];
        Word.Part = matchPartOfSpeach l.[3];
        Word.Gender = matchGender l.[4];
        Word.Forms = l.[5] }
    else
        None  
        
let someOrEmpty (x: 'T Option) =
    match x with
    | Some a -> sprintf "%A" a
    | None -> ""        

let toString (w:Word) =
    let res = ""
    [ w.Language; w.Spelling; w.Translation; someOrEmpty w.Part; someOrEmpty w.Gender; w.Forms ]
    |> Seq.fold (fun s x -> s + x + separator.[0].ToString( )) res                  
    
let readCsv filename = 
    File.ReadAllLines(filename, Text.Encoding.UTF8) |> Seq.choose(fromString)    

let writeCsv filename words =
    let lines = words |> Seq.map toString |> Seq.toArray
    File.WriteAllLines( filename, lines, Text.Encoding.UTF8 )    
    