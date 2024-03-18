// For more information see https://aka.ms/fsharp-console-apps
module Edulink.Program

open System
open System.Net.Http
open System.Net.Http.Headers
open System.Text
open Newtonsoft.Json
open Newtonsoft.Json.Linq

let baseUrl = "https://www14.edulinkone.com/api/?method="

let userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:123.0) Gecko/20100101 Firefox/123.0"


let uuid () =
    Guid.NewGuid().ToString()

let createRequestWithBaseUrl (method: string) (parameters: string) (url: string) =
    let request = new HttpRequestMessage ()
    request.Method <- HttpMethod.Post
    request.RequestUri <- Uri (url + method)
    request.Headers.Add ("X-API-Method", method)
    request.Headers.UserAgent.ParseAdd userAgent
    
    let content = $$"""{
    "jsonrpc": "2.0",
    "method": "{{method}}",
    "params": {
        {{parameters}}   
    },
    "uuid": "{{uuid ()}}",
    "id": "1"
}"""
    
    request.Content <- new StringContent (content, Encoding.UTF8, "application/json")
    
    request

let createRequest method parameters =
    createRequestWithBaseUrl method parameters baseUrl

let await =
    Async.AwaitTask >> Async.RunSynchronously

let sendRequest method parameters (client: HttpClient) =
    let response =
        createRequest method parameters
        |> client.Send
        
    await (response.Content.ReadAsStringAsync())

let sendAuthRequest method parameters token (client: HttpClient) =
    let request = createRequest method parameters
    request.Headers.Authorization <- AuthenticationHeaderValue ("Bearer", token)
    let response = client.Send request
    
    await (response.Content.ReadAsStringAsync())

let schoolId code =
    use client = new HttpClient ()
    
    let parameters = $"""
        "code": "{code}"
        """
    
    let response =
        createRequestWithBaseUrl "School.FromCode" parameters "https://provisioning.edulinkone.com/?method="
        |> client.Send
    
    let content = await (response.Content.ReadAsStringAsync()) |> JObject.Parse
        
    let success = content["result"].["success"].ToObject<bool>()
    
    if success then
        Some (content["result"].["school"].["school_id"].ToObject<int>())
    else
        None
    
let schoolName id =
    use client = new HttpClient ()
    
    let parameters = $"""
    "establishment_id": "{id}",
    "from_app": false
    """
    
    let response =
        sendRequest "EduLink.SchoolDetails" parameters client
        |> JObject.Parse
        
    response["result"].["establishment"].["name"].ToObject<string>()
    
let login username password : Auth =
    use client = new HttpClient ()
    
    let parameters = $$""""from_app": false,
        "ui_info": {
            "format": 2,
            "version": "5.0.70",
            "git_sha": "9c82c853dd440306800f155f15e876823e006490"
        },
        "fcm_token_old": "none",
        "device": {},
        "username": "{{username}}",
        "password": "{{password}}",
        "establishment_id": 46"""
       
    let content =
        sendRequest "EduLink.Login" parameters client
        |> JObject.Parse
        
    content["result"].["authtoken"].ToObject<string>(), content["result"].["user"].["id"].ToObject<string>()
    
[<EntryPoint>]
let main _ =
    printfn $"""{login "19eho" "hqijbmcohvfvndqbbuuond20080407()hqijbmcohvfvndqbbuuond20080407()"}"""
    
    schoolId "sn14je"
    |> function
        | Some id -> printfn $"Name: {schoolName id}"
        | _ -> ()
        
    0