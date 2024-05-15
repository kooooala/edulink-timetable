module Edulink.Calendar

open System
open System.IO
open System.Security.Cryptography
open System.Text

let generateLessonUid (day: Day) (period: Period) lesson =
    // should be unique to lesson
    (lesson.Teachers +
    lesson.TeachingGroup.Name +
    period.Name +
    period.StartTime.ToString() +
    period.EndTime.ToString() + 
    day.Date.ToString())
    |> Encoding.ASCII.GetBytes
    |> SHA1.HashData
    |> Convert.ToBase64String
    
    
let generateEmptyPeriodUid (day: Day) period =
    (period.StartTime.ToString() +
    period.EndTime.ToString() +
    day.Date.ToString())
    |> Encoding.ASCII.GetBytes
    |> SHA1.HashData
    |> Convert.ToBase64String
    
let writeDateTime (day: Day) period (sw: StreamWriter) =
    let offset = DateTime.Now - DateTime.UtcNow
    
    sw.WriteLine $"""DTSTAMP:{DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ")}"""
    sw.WriteLine $"""DTSTART:{(day.Date + period.StartTime.TimeOfDay - offset).ToString("yyyyMMddTHHmmssZ")}"""
    sw.WriteLine $"""DTEND:{(day.Date + period.EndTime.TimeOfDay - offset).ToString("yyyyMMddTHHmmssZ")}"""
    
let writeLesson (day: Day) period lesson (sw: StreamWriter) =
    let description = $"{lesson.TeachingGroup.Name}\n \\nTeacher: {lesson.Teachers}"
    
    sw.WriteLine $"UID:{generateLessonUid day period lesson}"
    sw.WriteLine $"LOCATION:{lesson.Room.Name}"
    sw.WriteLine $"SUMMARY:{period.Name}: {lesson.TeachingGroup.Subject}"
    sw.WriteLine $"DESCRIPTION:{description}"
    
    writeDateTime day period sw
    
let writeEmptyPeriod (day: Day) period (sw: StreamWriter) =
    sw.WriteLine $"UID:{generateEmptyPeriodUid day period}"
    sw.WriteLine $"SUMMARY:{period.Name}"
    sw.WriteLine $"DESCRIPTION:{period.Name}"
    
    writeDateTime day period sw
    
let writePeriod day period (sw: StreamWriter) =
    sw.WriteLine "BEGIN:VEVENT"
    
    let hasLesson =
        day.Lessons
        |> List.exists (fun i -> i.PeriodId = period.Id)
        
    if hasLesson then
        writeLesson day period (List.find (fun i -> i.PeriodId = period.Id) day.Lessons) sw
    else
        writeEmptyPeriod day period sw
    
    sw.WriteLine "END:VEVENT"
    
let writeFile weeks (fileName: string) =
    use sw = new StreamWriter (fileName)
    
    sw.WriteLine "BEGIN:VCALENDAR"
    sw.WriteLine "VERSION:2.0"
    sw.WriteLine "PRODID:-//EthanHo//"
    sw.WriteLine "METHOD:PUBLISH"

    printfn $"{weeks}"
    
    for week in weeks do
        for day in week.Days do
            for period in day.Periods do
                writePeriod day period sw 
    
    sw.WriteLine "END:VCALENDAR"
    
    sw.Flush ()
    sw.Close ()
    
let generateExamUid exam =
    exam.Board +
    exam.Level +
    exam.Code
    |> Encoding.ASCII.GetBytes
    |> SHA1.HashData
    |> Convert.ToBase64String
    
let writeExam exam (sw: StreamWriter) =
    sw.WriteLine "BEGIN:VEVENT"
    
    let description = $"{exam.Level} {exam.Board} - {exam.Title}\n \\nPaper Reference: {exam.Code}\n \\nRoom: {exam.Room} Seat: {exam.Seat}\n \\nDuration: {exam.Duration.Hours}hr {exam.Duration.Minutes}m"
    sw.WriteLine $"UID:{generateExamUid exam}"
    sw.WriteLine $"LOCATION:Room: {exam.Room} Seat: {exam.Seat}"
    sw.WriteLine $"SUMMARY:Exam: {exam.Title}"
    sw.WriteLine $"DESCRIPTION:{description}"
    
    let offset = DateTime.Now - DateTime.UtcNow
    sw.WriteLine $"""DTSTAMP:{DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ")}"""
    sw.WriteLine $"""DTSTART:{(exam.DateTime - offset).ToString("yyyyMMddTHHmmssZ")}"""
    sw.WriteLine $"""DTEND:{(exam.DateTime + exam.Duration - offset).ToString("yyyyMMddTHHmmssZ")}"""
    
    sw.WriteLine "END:VEVENT"
    
let writeExams (exams: Exam list) (fileName: string) =
    use sw = new StreamWriter (fileName)
    
    sw.WriteLine "BEGIN:VCALENDAR"
    sw.WriteLine "VERSION:2.0"
    sw.WriteLine "PRODID:-//EthanHo//"
    sw.WriteLine "METHOD:PUBLISH"
    
    for exam in exams do
        writeExam exam sw
        
    sw.WriteLine "END:VCALENDAR"
    
    sw.Flush ()
    sw.Close ()