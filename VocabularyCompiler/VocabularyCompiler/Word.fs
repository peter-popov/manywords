module Word

open System.Text.RegularExpressions

type PartOfSpeach = 
    | Noun
    | Verb
    | Adjective
    | Adverb
    | Pronoun
    | Preposition
    | Interjection
    | Conjunction
            
type Gender =
    | Femine
    | Musculin
    | Neutral
    
//
// Db record    
type Word = { Language: string;
              Spelling: string; 
              Translation: string; 
              Part: Option<PartOfSpeach>; 
              Gender: Option<Gender>; 
              Forms: string }
              
              
let (|Match|_|) pattern input =
    let m = Regex.Match(input, pattern, RegexOptions.IgnoreCase) in
    if m.Success then Some(true) else None

let matchPartOfSpeach (s:string) =
    match s.Trim() with
        | Match "(^v|\bverb)" _ -> Some(Verb)
        | Match "(^n|\bnoun)" _ -> Some(Noun)
        | Match "(^adj|\badj)" _ -> Some(Adjective)
        | Match "(^adv|\badv)" _ -> Some(Adverb)
        | Match "(prep)" _ -> Some(Preposition)
        | Match "(pronoun)" _ -> Some(Pronoun)
        | Match "(Interj)" _ -> Some(Preposition)
        | Match "(Conjun)" _ -> Some(Pronoun)
        | _ -> None   
        
let matchGender (s:string) =
    match s.Trim() with
        | Match "(^m|\bm)" _ -> Some(Musculin)
        | Match "(^f|\bf)" _ -> Some(Femine)
        | Match "(^n|\bn)" _ -> Some(Neutral)
        | _ -> None                              