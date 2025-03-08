using DragonBallKaiUBDLib;
using Mono.Options;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DragonBallKaiUBDUtility
{
    public class TopTextureCommand : Command
    {
        private string _inputDirectory, _outputDirectory;
        public TopTextureCommand() : base("toptext-dso", "Converts a DSO texture into a PNG")
        {
            Options = new()
            {
                { "i|input=", "Input DSO file or directory", i => _inputDirectory = i },
                { "o|output=", "Output PNG file or directory", o => _outputDirectory = o }
            };

            /*Options = new()
            {
                { "i|input=", "Input DSO file", i => _dsoFile = i },
                { "o|output=", "Output PNG file", o => _outputFile = o },
            };*/
        }

        public override int Invoke(IEnumerable<string> arguments)
        {
            Options.Parse(arguments);

            var dsoFiles = Directory.GetFiles(_inputDirectory, "*.dso");
            foreach (var dsoFile in dsoFiles)
            {
                var outputFileName = Path.GetFileNameWithoutExtension(dsoFile) + "complete.dso";
                var outputFilePath = Path.Combine(_outputDirectory, outputFileName);
                ProcessFile(dsoFile, outputFilePath);
            }

            //TopTexture dso = new(File.ReadAllBytes(_dsoFile));
            //File.WriteAllBytes(_outputFile, dso.Data);
            ////using FileStream fs = File.Create(_outputFile);
            ////dso.GetImage().Encode(fs, SKEncodedImageFormat.Png, 1);

            return 0;
        }

        public void ProcessFile(string inputFile, string outputFile)
        {
            TopTexture dso = new(File.ReadAllBytes(inputFile));
            File.WriteAllBytes(outputFile, dso.Data);
            //using FileStream fs = File.Create(outputFile);
            //dso.GetImage().Encode(fs, SKEncodedImageFormat.Png, 1);
        }
    }
}
