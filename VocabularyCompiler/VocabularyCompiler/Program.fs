// Learn more about F# at http://fsharp.net
module Main

open System
open System.IO
open System.Collections.Generic
open System.Reflection
open System.Text.RegularExpressions
open XlsReader
open Word       
open CsvStore
open WordsMatcher        


//
// Export words from excell
let exportExcell path output =
    let rec findFiles dir filter= 
        seq { yield! Directory.GetFiles(dir, filter)
              for subdir in Directory.GetDirectories(dir) do yield! findFiles subdir filter}
    let filter = "*.xls"
    findFiles path filter 
    |> Seq.collect readFile 
    |> writeCsv output 

//
// Find all words which have translation to al least 4 languages
type Stat = { Languages:HashSet<string>; Words:HashSet<string>; mutable Hits:int}    
let findImportantWords words =
    let stat = new Dictionary<string, Stat >()
    for (word : Word) in words do
        let keys = extractTranslations word.Translation
        for k in keys do
            let key = k.ToLower().Trim()        
            if not(stat.ContainsKey(key)) then
                stat.[key] <- { Stat.Languages = new HashSet<string>();
                                Stat.Words = new HashSet<string>(); 
                                Stat.Hits = 0 }
            ignore (stat.[key].Languages.Add(word.Language))
            ignore (stat.[key].Words.Add(word.Spelling))
            ignore (stat.[key].Hits <- stat.[key].Hits + 1)               

    stat.Values 
    |> Seq.choose (fun x -> if x.Languages.Count >= 4 then Some(x) else None)
    |> Seq.collect (fun x -> seq{ for v in x.Words do yield v } )
    |> Set.ofSeq



//let op1 =
//    let path = (System.Environment.GetCommandLineArgs().GetValue(1).ToString())    
//    exportExcell path "all_words.txt"
//
//let op2 =
//    let words = readCsv "all_words.txt" 
//                |> Seq.filter (fun x -> x.Part.IsSome )                    
//    let importantWords = findImportantWords words
//    printfn "%d words, %d important " (Seq.length words) importantWords.Count
//    words      
//    |> Seq.filter (fun x -> importantWords.Contains(x.Spelling) )
//    |> writeCsv "important_words.txt"  
    
let op3 =
    let words = readCsv "important_words.txt" 
    buildConnections words    
    for w in Seq.skip 855 words |> Seq.take 1 do        
        printfn "%A" w
        printfn "==========================="
        let connected = extractConnected w
        writeCsv "ich_connections.txt" connected  
        
        
op3          

