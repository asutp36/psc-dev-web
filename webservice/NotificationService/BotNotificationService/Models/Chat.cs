using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace BotNotificationService.Models
{
    public class Chat
    {
        public int id { get; set; }
        public string title { get; set; }
        //{ get 
        //    {
        //        //var options = new JsonSerializerOptions
        //        //{
        //        //    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
        //        //    WriteIndented = true
        //        //};
        //        //var decoded = System.Text.Json.JsonSerializer.Serialize(this.title, options);

        //        //return decoded;
        //        return this.title
        //    } 
        //    set { }
        //}
    }
}
