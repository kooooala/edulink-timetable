[<AutoOpen>]
module Edulink.Types

open System
open Newtonsoft.Json

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

type Auth = string * string

type Result<'a> =
    | Success of 'a 
    | Failure of string