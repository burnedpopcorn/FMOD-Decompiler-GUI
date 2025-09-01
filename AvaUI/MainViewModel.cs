using Cysharp.Diagnostics;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace FMOD_DecompilerUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    public Interaction<string, string> PickAFolder { get; } = new();

    public Interaction<string[], Unit> MsgBoxError { get; } = new();

    public Interaction<string[], Unit> MsgBoxInfo { get; } = new();

    public Interaction<string[], bool> MsgBoxYesNo { get; } = new();

    public Interaction<string, Unit> AddConsoleLine { get; } = new();


    [Reactive]
    public bool IsPathsReadOnly { get; set; } = false;

    [Reactive]
    public string BanksPath { get; set; } = string.Empty;

    [Reactive]
    public string OutputPath { get; set; } = string.Empty;
    [Reactive]
    public string ProjectName { get; set; } = string.Empty;

    [Reactive]
    public string ProgressText { get; set; } = "Idle";

    [Reactive]
    public int ProgressMaximum { get; set; } = 1;

    [Reactive]
    public int ProgressValue { get; set; } = 0;

    public Subject<string> ConsoleLine { get; } = new();

    public ReactiveCommand<Unit, Unit> PickBanksFolderCommand { get; }
    public ReactiveCommand<Unit, Unit> PickOutputFolderCommand { get; }

    public ReactiveCommand<Unit, Unit> StartExtractingCommand { get; }

    public MainViewModel()
    {
        this.PickBanksFolderCommand = ReactiveCommand.CreateFromTask(PickBanksFolder);
        this.PickOutputFolderCommand = ReactiveCommand.CreateFromTask(PickOutputFolder);

        this.StartExtractingCommand = ReactiveCommand.CreateFromTask(StartExtracting);

        this.StartExtractingCommand.ThrownExceptions.Subscribe(async ex =>
        {
            await MsgBoxError.Handle(new string[] {
                    "ERROR!",
                    $"Error: {ex.Message}" }).ToTask();
        });

        var curDir = Environment.CurrentDirectory;

        BanksPath = Path.Combine(curDir, "banks");
        OutputPath = Path.Combine(curDir, "output");

        if (Directory.Exists(BanksPath) == false)
            Directory.CreateDirectory(BanksPath);

        if (Directory.Exists(OutputPath) == false)
            Directory.CreateDirectory(OutputPath);

        ProjectName = "Generic-Project";
    }

    async Task PickBanksFolder()
    {
        var path = await PickAFolder.Handle("Select Banks Folder:").ToTask();

        if (path! == string.Empty)
            return;

        BanksPath = path;
    }

    async Task PickOutputFolder()
    {
        var path = await PickAFolder.Handle("Select Folder to Output Project:").ToTask();

        if (path! == string.Empty)
            return;

        OutputPath = path;
    }

    async Task StartExtracting()
    {
        if (Directory.Exists(BanksPath) == false)
        {
            await MsgBoxError.Handle(new string[] {
                    "ERROR!",
                    "Provided Banks Path doesn't exist!" }).ToTask();
            return;
        }

        IsPathsReadOnly = true;

        if (ProjectName == string.Empty)
        {
            var proceed = await MsgBoxYesNo.Handle(new string[] {
                    "Warning!",
                    "Project Name field is Empty! Do you still want to proceed?" }).ToTask();

            if (proceed == false)
            {
                IsPathsReadOnly = false;
                return;
            }
        }

        // Launch FMOD-Decompiler Console App
        var consoleappstart = ProcessX.StartAsync($"FMOD-Decompiler\\FMOD-Decompiler.exe --input {BanksPath} --output {OutputPath} --verbose --GUI");
        // Get Console Output
        await foreach (string consoleLine in consoleappstart)
        {
            await AddConsoleLine.Handle(consoleLine).ToTask();
        }

        IsPathsReadOnly = false;
    }
}
