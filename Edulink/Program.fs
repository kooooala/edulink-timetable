// For more information see https://aka.ms/fsharp-console-apps
module Edulink.Program

open System
open Edulink
open Calendar
    
let bind f opt =
    match opt with
    | Success x -> f x
    | Failure failure -> Failure failure
    
let getPassword () =
    let mutable password = String.Empty
    
    let mutable i = Console.ReadKey true
    
    if (i.Key = ConsoleKey.Backspace && password.Length > 0) then
            Console.Write "\b \b"
            password <- password[0..(password.Length - 2)]
        elif not (Char.IsControl(i.KeyChar)) then
            Console.Write "*"
            password <- password + string i.KeyChar
    
    while i.Key <> ConsoleKey.Enter do
        i <- Console.ReadKey true
        
        if (i.Key = ConsoleKey.Backspace && password.Length > 0) then
            Console.Write "\b \b"
            password <- password[0..(password.Length - 2)]
        elif not (Char.IsControl(i.KeyChar)) then
            Console.Write "*"
            password <- password + string i.KeyChar
                
    Console.WriteLine ()
    password            
    
[<EntryPoint>]
let main _ =
    printf "School: "
    let school = Console.ReadLine()
    
    printf "Username: "
    let username = Console.ReadLine()
    printf "Password: "
    let password = getPassword ()
    
    let result =
        schoolId school
        |> bind (login username password)
        |> bind timetable
        
    let exams =
        schoolId school
        |> bind (login username password)
        |> bind exams
        
    printfn $"{exams}"
        
    match result with
    | Success timetable ->
        printf "Output file name (Press enter to use default): "
        let fileName = Console.ReadLine()
        writeFile timetable (
        (if fileName = "" then $"Timetable-{DateTime.Now.ToShortDateString().Replace('/', '-')}"
        else fileName) + ".ics")
    | Failure error -> eprintfn $"Error: {error}"
        
    match exams with
    | Success exams ->
        printf "Exam timetable file name (Press enter to use default): "
        let fileName = Console.ReadLine()
        writeExams exams (
        (if fileName = "" then $"ExamTimetable-{DateTime.Now.ToShortDateString().Replace('/', '-')}"
        else fileName) + ".ics")
    | Failure error -> eprintfn $"Error: {error}"
        
    0