open System
open Argu

type CLIArg = CustomCommandLineAttribute

type CLIArguments =
    | [<CLIArg "demo">] Demo
    | Project of name:string   
with 
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Project _ -> "Specify the project you want to change too."
            | Demo -> "Sets it to the demo project"

let defaultForeground = Console.ForegroundColor
let defaultBackground = Console.BackgroundColor

let cyan = ConsoleColor.Cyan

let writeln color (msg: string) =
    Console.ForegroundColor <- color
    Console.WriteLine msg
    Console.ForegroundColor <- defaultForeground

let setRegistryValue key = 
    use path = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\EGEMIN\JB_MIS")
    match path with
    | null -> failwith("Access failed to registry: hklm\\SOFTWARE")
    | akey -> 
        match akey.GetValue("CITYNAME", null) with
        | null -> failwith("Path not found: " + key)
        | gotkey -> gotkey.ToString()

let parseCommand args = 
    let parser = ArgumentParser.Create<CLIArguments>()
    let result = parser.Parse(args)
    result

let changeProject (args: string []) =
    let results = parseCommand args
    let rec processing (res: ParseResults<_>) =
        match res.GetAllResults() with 
        | [cmd] -> match cmd with
                   | Project prt -> writeln cyan prt
                   | Demo -> writeln cyan "demo"
        | _ -> writeln cyan "all the rest"
    processing results


[<EntryPoint>]
let main argv = 
    match argv with
    | [||] -> writeln cyan "this is the first one"
    | _ -> setRegistryValue "TEST" |> writeln cyan
    0
