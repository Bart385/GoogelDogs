using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace DiffMatchPatch
{
    public class Test
    {
        public static void Main(string[] args)
        {
            string oldText = "Goodbye World!";
            string newText = "Hello World!";
            diff_match_patch dmp = new diff_match_patch();
            List<Diff> diff = dmp.diff_main(oldText, newText);
            // Result: [(-1, "Hell"), (1, "G"), (0, "o"), (1, "odbye"), (0, " World.")]
            dmp.diff_cleanupSemantic(diff);
            // Result: [(-1, "Hello"), (1, "Goodbye"), (0, " World.")]
            for (int i = 0; i < diff.Count; i++)
            {
                Console.WriteLine(diff[i]);
            }

            List<Patch> patches = dmp.patch_make(oldText, diff);
            var result = dmp.patch_apply(patches, oldText);
            Console.WriteLine(result[0]);

            dynamic json = new
            {
                sender = "Me",
                diffs = diff
            };

            string serialized = JsonConvert.SerializeObject(json);
            dynamic deserialized = JsonConvert.DeserializeObject(serialized);

            Console.WriteLine(deserialized);
        }
    }
}