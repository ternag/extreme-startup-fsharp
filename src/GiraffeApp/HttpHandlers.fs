namespace GiraffeApp

open Microsoft.Extensions.Logging

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open FSharp.Control.Tasks.V2.ContextInsensitive
    open Giraffe
    open GiraffeApp.Models
    open System.Text.RegularExpressions
    open System

    let handleGetHello =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = "Hello world, from Giraffe!"
                }
                return! json response next ctx
            }

    let pipelineLogger =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let logger = ctx.GetLogger("RequestUrl")
            let url = ctx.GetRequestUrl()
            logger.LogInformation url
            next ctx

    let (|Int|_|) (str:string) =
       match System.Int64.TryParse(str) with
       | (true,int) -> Some(int)
       | _ -> None

    let toInt (str:string) = 
        match str with
        | Int i -> i
        | _ -> 0L

    let fromInt64ToFloat (n:int64) =
        Convert.ToDouble n

    let isSquareAndCube num = 
        let s = seq [1L; 64L; 729L; 4096L; 15625L; 46656L; 117649L; 262144L; 531441L]
        Seq.exists (fun elm -> elm = num) s

    let isPrime (n:int64) =
        match n with
        | _ when n > 3L && (n % 2L = 0L || n % 3L = 0L) -> false
        | _ ->
            let maxDiv = int64(System.Math.Sqrt(float n)) + 1L
            let rec f d i = 
                if d > maxDiv then 
                    true
                else
                    if n % d = 0L then 
                        false
                    else
                        f (d + i) (6L - i)     
            f 5L 2L        

    let (|DetectAddition|_|) q = 
        let m = Regex.Match(q, @"what is (\d+) plus (\d+)")
        if m.Success then Some (m.Groups.[1].Value |> toInt, m.Groups.[2].Value |> toInt) else None

    let (|DetectPlusPlus|_|) q = 
        let m = Regex.Match(q, @"what is (\d+) plus (\d+) plus (\d+)")
        if m.Success then Some (m.Groups.[1].Value |> toInt, m.Groups.[2].Value |> toInt, m.Groups.[3].Value |> toInt) else None

    let (|DetectSubtraction|_|) q = 
        let m = Regex.Match(q, @"what is (\d+) minus (\d+)")
        if m.Success then Some (m.Groups.[1].Value |> toInt, m.Groups.[2].Value |> toInt) else None

    let (|DetectMultiplication|_|) q = 
        let m = Regex.Match(q, @"what is (\d+) multiplied by (\d+)")
        if m.Success then Some (m.Groups.[1].Value |> toInt, m.Groups.[2].Value |> toInt) else None

    let (|DetectFibonacci|_|) q = 
        let m = Regex.Match(q, @"what is the (\d+)(?:st|nd|rd|th) number in the Fibonacci sequence")
        if m.Success then Some (m.Groups.[1].Value |> toInt) else None

    let (|DetectPower|_|) q = 
        let m = Regex.Match(q, @"what is (\d+) to the power of (\d+)")
        if m.Success then Some (m.Groups.[1].Value |> toInt, m.Groups.[2].Value |> toInt) else None


    let fibonacci n : int64 =
      let rec f a b n =
        match n with
        | 0L -> a
        | 1L -> b
        | n -> (f b (a + b) (n - 1L))
      f (0L) (1L) n

    let answerQuestion (q:string) = 
        let split = q.Split(":") 
        match split.[1] with
        | "what is your name" -> "T"
        | DetectPlusPlus (x,y,z) -> sprintf "%i" (x+y+z)
        | DetectAddition (x,y) -> sprintf "%i" (x+y)
        | DetectSubtraction (x,y) -> sprintf "%i" (x-y)
        | DetectMultiplication (x,y) -> sprintf "%i" (x*y)
        | DetectPower (x,y) -> sprintf "%.0f" (Math.Pow(fromInt64ToFloat(x), fromInt64ToFloat(y)))
        | DetectFibonacci (x) -> fibonacci x |> sprintf "%i"
        | " which of the following numbers is the largest" -> split.[2].Split(",") |> Array.map(toInt) |> Array.max |> sprintf "%i"
        | " which of the following numbers is both a square and a cube" -> split.[2].Split(",") |> Array.map(toInt) |> Array.filter(isSquareAndCube) |> (fun elms -> if(Array.isEmpty elms) then "" else String.Join(", ", elms))
        | " which of the following numbers are primes" ->  split.[2].Split(",") |> Array.map(toInt) |> Array.filter(isPrime) |> (fun elms -> if(Array.isEmpty elms) then "" else String.Join(", ", elms))
        | " who played James Bond in the film Dr No" -> "Sean Connery"
        | " what colour is a banana" -> "Yellow"
        | " which year was Theresa May first elected as the Prime Minister of Great Britain" -> "2016"
        | " which city is the Eiffel tower in" -> "Paris"
        | _ -> ""


    let handleQuestion : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let logger = ctx.GetLogger("question")
            let question = ctx.TryGetQueryStringValue "q"
            match question with
                | None   -> logger.LogError "q not defined"
                | Some q -> logger.LogInformation q
            let answer =
                match question with
                | None   -> "q not defined"
                | Some q -> answerQuestion q
            let logger = ctx.GetLogger("answer")
            logger.LogInformation answer |> ignore
            text answer next ctx

