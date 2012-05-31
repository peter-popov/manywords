module WordsMatcher

open System
open System.IO
open System.Collections.Generic
open System.Text.RegularExpressions
open Word       


let extractTranslations s =
    let s = Regex.Replace(s, @"(\d\.|\d\)|;)", ",")
    let s = Regex.Replace(s, @"(\(\w*\))", "")
    let s = Regex.Replace(s, @"\&quot", "" )
    s.Split([|','|]) 
    |> Seq.filter( fun x -> x.ToLower().Trim().Length > 0 ) 
    |> Seq.distinct


let addConnection (key:'k) (value:'v) (dict:Dictionary<'k, List<'v>>) =
    if not(dict.ContainsKey(key)) then dict.[key] <- new List<'v>()
    dict.[key].Add(value)    

let getValues (key:'k) (dict:Dictionary<'k, List<'v>>) =
    if dict.ContainsKey(key) then 
        seq{for v in dict.[key] do yield v}
    else
        Seq.empty    


let lng_en = new Dictionary<Word, List<string>>()
let en_lng = new Dictionary<string, List<Word>>()

let buildConnections words =
    for word in words do
        let translations = extractTranslations word.Translation
        for t in translations do
            let key = t.ToLower().Trim()    
            addConnection key word en_lng
            addConnection word key lng_en
    
let cmp (w1:Word) (w2:Word) =
    ( w1.Part.IsNone || w2.Part.IsNone || (w1.Part.Value = w2.Part.Value) )
    
let rec deep_matches start (stack:HashSet<Word>) =    
    ignore(stack.Add(start))
    for t in getValues start lng_en do
        for w in getValues t en_lng do
            if not(stack.Contains(w)) && (cmp start w) then                               
                deep_matches w stack                         
            
let extractConnected word =
    let values = new HashSet<Word>()
    ignore(values.Add(word))
    for t in getValues word lng_en do
        for w in getValues t en_lng do
            if not(values.Contains(w)) && (cmp word w) then
                ignore(values.Add(w))
    values
                
                
//let extractLanguage words language =
//    words 
//    |> Seq.filter( fun x -> x.Language.Equals(language, StringComparison.InvariantCultureIgnoreCase) )            
    

    

            