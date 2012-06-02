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



let sieveOut input output =
    let words = readCsv input
                |> Seq.filter (fun x -> x.Part.IsSome )                    
    let importantWords = findImportantWords words
    printfn "%d words, %d important " (Seq.length words) importantWords.Count
    words      
    |> Seq.filter (fun x -> importantWords.Contains(x.Spelling) )
    |> writeCsv output 
    
let valueOrNothing (key:'k) (dict:Dictionary<'k,string>) =
    if not(dict.ContainsKey(key)) then
        ""
    else
        dict.[key]    

let translationsToCsv w lng translations =
    let mutable s = w.Spelling;
    for l in lng do
        s <- s + "$" + valueOrNothing l translations 
    s           
    
let extractLanguage language input output =
    let words = readCsv input 
    let languages = buildConnections words
    let lines = Array.ofList [ for w in words do
                                    if w.Language = language then
                                        let connected = extractConnected w
                                        let translations = extractMultilang w connected
                                        let srvline = sprintf "%s$%s" ( someOrEmpty w.Part ) ( someOrEmpty w.Gender )
                                        let translation = translationsToCsv w languages translations
                                        yield (translation + "$" + srvline) ]                           
    File.WriteAllLines( output, lines, Text.Encoding.UTF8 )        
        
//exportExcell (System.Environment.GetCommandLineArgs().GetValue(1).ToString()) "all_words.txt"        

//sieveOut "all_words.txt" "important_words.txt" 
                  

extractLanguage "German" "important_words.txt" "german.txt"