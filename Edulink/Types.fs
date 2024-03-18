[<AutoOpen>]
module Edulink.Types

open System

type Period = {
    Id: int
    Name: string
    Start: DateTime
    End: DateTime
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
    PeriodId: int
    Room: Room
    TeachingGroup: TeachingGroup
    Teachers: string
}

type Day = {
    Date: DateTime
    Periods: Period list
    Lessons: Lesson list
}

type Week = Day list

type Weeks = Week list

type Auth = string * string