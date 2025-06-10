open System.IO

/// Parses the specified file and returns a list of tuples representing blocks.
/// Each tuple: (sentence, parseLine, posLine)
let parseFile (fileName: string) =
    let lines = File.ReadAllLines(fileName) |> Array.toList

    // Helper to process lines into blocks
    let rec parseBlocks acc rest =
        match rest with
        | (line: string) :: _ when line.StartsWith("Analyzing file: ") ->
            // Skip the "Analyzing file: ..." line and continue
            parseBlocks acc (List.tail rest)
        | sentence :: parseLine :: posLine :: "" :: tail when parseLine.StartsWith("(") ->
            let block = (sentence, parseLine, posLine)
            parseBlocks (block :: acc) tail
        | _ :: tail ->
            parseBlocks acc tail
        | [] ->
            List.rev acc

    parseBlocks [] lines

let fileName = fsi.CommandLineArgs[1]
printfn "Parsing %s." fileName
let blocks = parseFile fileName

let removeInsideQuotes (s: (string * string) seq) =
    let rec remove acc rest =
        match rest with
        | [] -> List.rev acc
        | (_, "\"") :: xs ->
            remove (acc) (skip [] xs)
        | x :: xs -> remove (x :: acc) xs
    and skip acc rest =
        match rest with
        | [] -> List.rev acc
        | (_, "\"") :: xs ->
            ("QUOTED_STRING", "QUOTED_STRING") :: xs
        | x :: xs -> skip (x :: acc) xs
    remove [] (s |> Seq.toList)

let removeInsideParens (s: (string * string) seq) =
    let rec remove acc rest =
        match rest with
        | [] -> List.rev acc
        | (_, "(") :: xs ->
            remove (acc) (skip xs)
        | x :: xs -> remove (x :: acc) xs
    and skip rest =
        match rest with
        | [] -> []
        | (_, ")") :: xs ->
            xs
        | _ :: xs -> skip xs
    remove [] (s |> Seq.toList)

let isAdjOrVerb (word: string) =
    word = "cached" || word = "modified" || word = "formatted"
        || word = "leading" || word = "offset" || word = "reserved" || word = "left"
        || word = "boxWhenRestored" || word = "handle" || word = "selected"
        || word = "colorized" || word = "read-only" || word = "normalized"
        || word = "left-top"

let isAdjOrAdv (word: string) =
    word = "first"

let isNounNotAVerb word =
    word <> "times"

let detectVariables (s: (string * string) seq) =
    let rec remove acc rest =
        match rest with
        | [] -> List.rev acc
        // DET NOUN NOUN PART NOUN NOUN PART ADJ PART NOUN NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("ADJ", p8)
            :: ("PART", p9) :: ("NOUN", p10) :: ("NOUN", p11)
            :: ("PART", p12) :: ("NOUN", p13) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9} {p10} {p11} {p12} {p13}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART NOUN NOUN PART NOUN PART NOUN NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8)
            :: ("PART", p9) :: ("NOUN", p10) :: ("NOUN", p11)
            :: ("PART", p12) :: ("NOUN", p13) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9} {p10} {p11} {p12} {p13}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART NOUN NOUN PART NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8)
            :: ("PART", p9) :: ("NOUN", p10) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9} {p10}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART NOUN NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART NOUN NOUN PART ADJ
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("ADJ", p8) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART NOUN PART NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5)
            :: ("PART", p6) :: ("NOUN", p7)
            :: ("PART", p8) :: ("NOUN", p9) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART NOUN PART NOUN PART ADJ
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5)
            :: ("PART", p6) :: ("NOUN", p7)
            :: ("PART", p8) :: ("ADJ", p9) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5)
            :: ("PART", p6) :: ("NOUN", p7) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART NOUN PART ADJ NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5)
            :: ("PART", p6) :: ("ADJ", p7) :: ("NOUN", p8) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART ADJ NOUN PART NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("ADJ", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8) :: ("NOUN", p9) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART ADJ NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("ADJ", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART ADJ NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("ADJ", p5) :: ("NOUN", p6) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART ADJ VERB? NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("ADJ", p5) :: ("VERB", p6) :: ("NOUN", p7) :: xs when isAdjOrVerb p6 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART ADJ VERB?
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("ADJ", p5) :: ("VERB", p6) :: xs when isAdjOrVerb p6 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART ADJ
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("ADJ", p5) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART NOUN NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5) :: ("NOUN", p6) :: ("NOUN", p7) :: xs when isNounNotAVerb p7 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5) :: ("NOUN", p6) :: xs when isNounNotAVerb p6 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART NOUN PART VERB? NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5)
            :: ("PART", p6) :: ("VERB", p7) :: ("NOUN", p8) :: xs when isAdjOrVerb p7 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART NOUN PART
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5)
            :: ("PART", p6) :: xs when p6 <> "to" ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART NOUN VERB?
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5) :: ("VERB", p6) :: xs when isAdjOrVerb p6 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART JJS NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("JJS", p5) :: ("NOUN", p6) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("NOUN", p5) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART SYM
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("SYM", p5) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART ADV
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("ADV", p5) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN PART VERB?
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3)
            :: ("PART", p4) :: ("VERB", p5) :: xs when isAdjOrVerb p5 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN NOUN PART NOUN NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6) :: ("NOUN", p7) :: ("NOUN", p8) :: xs when isNounNotAVerb p8 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN NOUN PART NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6) :: ("NOUN", p7) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3) :: ("NOUN", p4) :: ("NOUN", p5) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN NOUN VERB?
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3) :: ("NOUN", p4) :: ("VERB", p5) :: xs when isAdjOrVerb p5 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3) :: ("NOUN", p4) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN PART NOUN NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8) :: ("NOUN", p9)
            :: ("PART", p10) :: ("NOUN", p11) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9} {p10} {p11}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN PART NOUN PART NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8)
            :: ("PART", p9) :: ("NOUN", p10) :: ("NOUN", p11) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9} {p10} {p11}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN PART NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8)
            :: ("PART", p9) :: ("NOUN", p10) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9} {p10}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN PART NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8) :: ("NOUN", p9) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN PART NOUN VERB? NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8) :: ("VERB", p9) :: ("NOUN", p10) :: xs when isAdjOrVerb p9 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9} {p10}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN PART NOUN VERB?
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8) :: ("VERB", p9) :: xs when isAdjOrVerb p9 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN PART NOUN ADJ NOUN PART NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8) :: ("ADJ", p9) :: ("NOUN", p10)
            :: ("PART", p11) :: ("NOUN", p12)
            :: ("PART", p13) :: ("NOUN", p14) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9} {p10} {p11} {p12} {p13} {p14}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN PART NOUN ADJ NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8) :: ("ADJ", p9) :: ("NOUN", p10) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9} {p10}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN PART NOUN ADJ
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8) :: ("ADJ", p9) :: xs when isAdjOrVerb p9 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("NOUN", p8) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN PART VERB? NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("VERB", p8) :: ("NOUN", p9) :: xs when isAdjOrVerb p8 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN PART VERB?
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("VERB", p8) :: xs when isAdjOrVerb p8 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN PART ADJ NOUN PART NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("ADJ", p8) :: ("NOUN", p9)
            :: ("PART", p10) :: ("NOUN", p11)
            :: ("PART", p12) :: ("NOUN", p13) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9} {p10} {p11} {p12} {p13}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN PART ADJ
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6)
            :: ("PART", p7) :: ("ADJ", p8) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6) :: ("NOUN", p7)
            :: ("PART", p8) :: ("NOUN", p9) :: xs when isNounNotAVerb p9  ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6) :: ("NOUN", p7) :: xs when isNounNotAVerb p7 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART ADJ PART NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("ADJ", p6)
            :: ("PART", p7) :: ("NOUN", p8)
            :: ("PART", p9) :: ("NOUN", p10) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9} {p10}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART ADJ PART NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("ADJ", p6)
            :: ("PART", p7) :: ("NOUN", p8) :: ("NOUN", p9) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART ADJ PART NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("ADJ", p6)
            :: ("PART", p7) :: ("NOUN", p8) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART ADJ NOUN PART NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("ADJ", p6) :: ("NOUN", p7)
            :: ("PART", p8) :: ("NOUN", p9)
            :: ("PART", p10) :: ("NOUN", p11) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9} {p10} {p11}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART ADJ NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("ADJ", p6) :: ("NOUN", p7)
            :: ("PART", p8) :: ("NOUN", p9) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART ADJ NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("ADJ", p6) :: ("NOUN", p7) :: ("NOUN", p8) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART ADJ NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("ADJ", p6) :: ("NOUN", p7) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART ADJ ADJ NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("ADJ", p6) :: ("ADJ", p7) :: ("NOUN", p8) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART ADJ
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("ADJ", p6) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("NOUN", p6) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART VERB NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("VERB", p6) :: ("NOUN", p7) :: ("NOUN", p8) :: xs when isAdjOrVerb p6 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART VERB NOUN 
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("VERB", p6) :: ("NOUN", p7) :: xs when isAdjOrVerb p6 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART VERB
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("VERB", p6) :: xs when isAdjOrVerb p6 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART ADV
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("ADV", p6) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN PART SYM
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4)
            :: ("PART", p5) :: ("SYM", p6) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN NOUN PART NOUN NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4) :: ("NOUN", p5)
            :: ("PART", p6) :: ("NOUN", p7) :: ("NOUN", p8) :: ("NOUN", p9) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p9}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN NOUN PART NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4) :: ("NOUN", p5)
            :: ("PART", p6) :: ("NOUN", p7) :: ("NOUN", p8) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4) :: ("NOUN", p5)
            :: ("PART", p6) :: ("NOUN", p7) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN NOUN PART ADJ NOUN NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4) :: ("NOUN", p5)
            :: ("PART", p6) :: ("ADJ", p7):: ("NOUN", p8):: ("NOUN", p9):: ("NOUN", p10) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7} {p8} {p8} {p10}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN NOUN PART ADJ
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4) :: ("NOUN", p5)
            :: ("PART", p6) :: ("ADJ", p7) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN NOUN PART ADV
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4) :: ("NOUN", p5)
            :: ("PART", p6) :: ("ADV", p7) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN NOUN PART VERB?
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4) :: ("NOUN", p5)
            :: ("PART", p6) :: ("VERB", p7) :: xs when isAdjOrVerb p7 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6} {p7}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4) :: ("NOUN", p5) :: ("NOUN", p6) :: xs when isNounNotAVerb p6 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5} {p6}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4) :: ("NOUN", p5) :: xs when isNounNotAVerb p5 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4} {p5}")
            remove (replacement :: acc) xs
        // DET NOUN PART NOUN
        | ("DET", p1) :: ("NOUN", p2)
            :: ("PART", p3) :: ("NOUN", p4) :: xs when isNounNotAVerb p4 ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3} {p4}")
            remove (replacement :: acc) xs
        // DET NOUN NOUN
        | ("DET", p1) :: ("NOUN", p2) :: ("NOUN", p3) :: xs ->
            let replacement = ("VARIABLE", $"{p1} {p2} {p3}")
            remove (replacement :: acc) xs
        | x :: xs -> remove (x :: acc) xs
    remove [] (s |> Seq.toList)

let splitSentences (s: (string * string) seq) =
    let rec remove acc rest =
        match rest with
        | [] -> List.rev acc
        | (_, ",") :: xs ->
            remove ([] :: acc) (xs)
        | (_, ";") :: xs ->
            remove ([] :: acc) (xs)
        | x :: xs ->
            match acc with
            | [] -> remove ([[x]]) xs
            | [x'] -> remove ([x :: x']) xs
            | x' :: acc' ->
                remove ((x :: x') :: acc') xs
    remove [] (s |> Seq.toList) |> Seq.map (fun x -> x |> List.rev)


let blocks2 =
    blocks
    |> List.filter (fun (sentence, _, _) -> not (sentence.StartsWith("intel $")))
    |> List.filter (fun (sentence, _, _) -> not (sentence.StartsWith("to ")))
    |> List.filter (fun (sentence, _, _) -> not (sentence.StartsWith("To ")))
    |> Seq.map (fun (s, p, pos) ->
    let z = p.Trim().Split(") (")
    z[0] <- z[0].TrimStart('(')
    z[z.Length - 1] <- z[z.Length - 1].TrimEnd(')')
    let z =
        z |> Seq.map (fun item ->
            let parts = item.Split(' ')
            (parts[0], parts[1]))
        |> Seq.toArray
    (s, z, pos))
    |> Seq.map (fun (s, p, pos) ->
        //printfn "%A" p
        //printfn "------------------------"
        let p = removeInsideQuotes p
        //printfn "%A" p
        //printfn "------------------------"
        let p = removeInsideParens p
        //printfn "%A" p
        //printfn "------------------------"
        let p = detectVariables p
        //printfn "%A" p
        //printfn "------------------------"
        (s, p, pos))
    |> Seq.map (fun (s, p, pos) ->
        let p =
            splitSentences p
            |> Seq.map (fun sentence -> (s, sentence, pos))
        //printfn "%A" p
        //printfn "------------------------"
        p)
    |> Seq.collect id
    |> Seq.map (fun (s, p, pos) ->
        let pos = p |> Seq.map fst |> String.concat " "
        (s, p, pos))

let items =
    blocks2
    |> Seq.map (fun (s, p, pos) -> (pos, p))
    |> Seq.groupBy (fun (pos, p) -> pos)
    |> Seq.map (fun (key, items) ->
        let items = items |> Seq.map snd |> Seq.toArray |> Array.distinct
        (key, items |> Array.length, items))
    |> Seq.sortByDescending (fun (key, qty, sentences) -> qty)
    |> Seq.toList

items |> Seq.iter (fun (key, count, sentences) -> printfn "%d|%s|%A" count key sentences)
//items |> Seq.iter (fun (key, count, sentences) -> printfn "%d|%s" count key)
//printfn "Parsed %A blocks." items
