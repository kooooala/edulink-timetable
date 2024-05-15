[<AutoOpen>]
module Edulink.Types

open System
open System.Text.RegularExpressions
open Newtonsoft.Json
open Newtonsoft.Json.Linq

type Period = {
    Id: int
    Name: string
    
    [<JsonProperty("start_time")>]
    StartTime: DateTime
    [<JsonProperty("end_time")>]
    EndTime: DateTime
    Empty: bool
}

type Room = {
    Name: string
    Moved: bool
}

type TeachingGroup = {
    Name: string
    Subject: string
}

type Lesson = {
    [<JsonProperty("period_id")>]
    PeriodId: int
    Room: Room
    [<JsonProperty("teaching_group")>]
    TeachingGroup: TeachingGroup
    Teachers: string
}

type Day = {
    Date: DateTime
    Periods: Period list
    Lessons: Lesson list
}

type Week = {
    Name: string
    [<JsonProperty("is_current")>]
    IsCurrent: bool
    Days: Day list
}

type Weeks = Week list

type TimeSpanConverter() =
    inherit JsonConverter()
    
    override _.WriteJson (_, _, _) =
         raise (NotImplementedException ())

    override this.ReadJson (reader, objectType, existingValue, serializer) =
        let jsonObject = reader.Value.ToString()
        
        let regex = Regex ("(?<hour>[0-9]+)hr (?<minutes>0?[0-9]|[1-5][0-9])m", RegexOptions.IgnoreCase)
        let result = regex.Match(jsonObject)
        let hour = result.Groups["hour"].Value |> int
        let minutes = result.Groups["minutes"].Value |> int
        
        TimeSpan.FromMinutes (hour * 60 + minutes |> float)
        
        
    override this.CanConvert (objectType) =
        true
        
type Exam = {
    DateTime: DateTime
    Board: string
    Level: string
    Code: string
    Title: string
    Room: string
    Seat: string
    [<JsonConverter(typeof<TimeSpanConverter>)>]
    Duration: TimeSpan
}

type Auth = string * string

type Result<'a> =
    | Success of 'a 
    | Failure of string
    
