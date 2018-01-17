using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommandDotNet.Attributes;
using Newtonsoft.Json;

namespace CommandDotNet.Example
{
    [ApplicationMetadata(Name = "SampleAppName", Description = "Sample application for demonstation", ExtendedHelpText = "this is extended help text")]
    public class MyApplication
    {
        private readonly bool _cjumped;
        private readonly string _clevel;
        private readonly int? _cfeets;
        private readonly IEnumerable<string> _cfriends;
        private readonly double _cheight;
        private readonly bool? _clog;
        private readonly string _cpassword;
        private readonly int _ctimes;
        private readonly string _cauthor;

        public MyApplication(
            [Option(Description = "c did someone jump?")]
            bool cjumped, 
            
            string clevel, 
            
            int? cfeets, 
            
            IEnumerable<string> cfriends, 
            
            double cheight, 
            
            bool? clog,
            
            string cpassword,
            
            int ctimes = 0, 
            
            string cauthor = "c john")
        {
            _cjumped = cjumped;
            _clevel = clevel;
            _cfeets = cfeets;
            _cfriends = cfriends;
            _cheight = cheight;
            _clog = clog;
            _cpassword = cpassword;
            _ctimes = ctimes;
            _cauthor = cauthor;
        }
        
        [ApplicationMetadata(Description = "m makes someone jump", Name = "JUMP")]
        public void Jump(
            [Option(Description = "m did someone jump?")]
            bool mjumped, 
            
            string mlevel, 
            
            int? mfeets, 
            
            double mheight, 
            
            bool? mlog,
            
            string mpassword,
            
            int mtimes = 0, 
            
            string mauthor = "m john")
        {

            Console.WriteLine("Constructor params");
            Console.WriteLine(JsonConvert.SerializeObject(new
            {
                cjumped = _cjumped,
                clevel = _clevel,
                cfeets = _cfeets,
                cfriends = _cfriends,
                cheight = _cheight,
                clog = _clog,
                cauthor = _cauthor,
                ctimes = _ctimes
            }, Formatting.Indented));
            
            
            Console.WriteLine("Method params");
            Console.WriteLine(JsonConvert.SerializeObject(new
            {
                mjumped,
                mlevel,
                mfeets,
                mpassword,
                mheight,
                mlog,
                mauthor,
                mtimes
            }, Formatting.Indented));
        }
    }
}