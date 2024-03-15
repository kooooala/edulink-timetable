// For more information see https://aka.ms/fsharp-console-apps

open System
open System.Net.Http
open System.Text
open System.Text.Unicode
open Newtonsoft.Json
open Newtonsoft.Json.Linq

let baseUrl = "https://www14.edulinkone.com/api/?method="

let userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:123.0) Gecko/20100101 Firefox/123.0"

let createRequest method content =
    let request = new HttpRequestMessage ()
    request.Method <- HttpMethod.Post
    request.RequestUri <- Uri (baseUrl + method)
    request.Headers.Add ("X-API-Method", method)
    request.Headers.UserAgent.ParseAdd userAgent
    
    request.Content <- new StringContent (content, Encoding.UTF8, "application/json")
    
    request

let await =
    Async.AwaitTask >> Async.RunSynchronously

let uuid () =
    Guid.NewGuid().ToString()

let login username password =
    use client = new HttpClient()
    
    let body = $$"""
{
    "jsonrpc": "2.0",
    "method": "EduLink.Login",
    "params": {
        "from_app": false,
        "ui_info": {
            "format": 2,
            "version": "5.0.70",
            "git_sha": "9c82c853dd440306800f155f15e876823e006490"
        },
        "fcm_token_old": "none",
        "device": {},
        "username": "{{username}}",
        "password": "{{password}}",
        "establishment_id": 46
    },
    "uuid": "{{uuid ()}}",
    "id": "1"
}"""
        
    let response =
        createRequest "EduLink.Login" body 
        |> client.Send
        
    let content =
        await (response.Content.ReadAsStringAsync ())
        |> JsonConvert.DeserializeObject<JToken>
        
    content["result"].["authtoken"] |> string
    
[<EntryPoint>]
let main _ =
    login "19eho" "hqijbmcohvfvndqbbuuond20080407()hqijbmcohvfvndqbbuuond20080407()"
    |> printfn "%s"
    0