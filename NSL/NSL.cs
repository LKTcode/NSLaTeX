using System.Collections.Generic;
using System.Text;

namespace NSLaTeX.Synctax
{
    public class Sync
    {
        public string Input { get; set; }
        public Sync(string input)
        {
            if (input == null) return;

            Input = input;
        }

        public List<ENV> FindAllENV(string ENVName)
        {
            var results = new List<ENV>();
            int start = 0;
            string beginENV = $"\\begin{{{ENVName}}}";
            string endENV = $"\\end{{{ENVName}}}";

            while ((start = Input.IndexOf(beginENV, start)) != -1)
            {
                int end = Input.IndexOf(endENV, start);
                if (end == -1)
                    break;

                int contentStart = start + beginENV.Length;
                int contentEnd = end;

                if (contentStart <= Input.Length && contentEnd > contentStart)
                {
                    string content = Input.Substring(contentStart, contentEnd - contentStart);
                    string FullENV = $"\\begin{{{ENVName}}}\n{content.Trim()}\n\\end{{{ENVName}}}";
                    results.Add(new ENV() { Name = ENVName, Content = content, All=FullENV });
                }

                start = end + endENV.Length;
            }
            return results;
        }

        public List<CMD> FindAllCMD(string CMDName)
        {
            string input = Input;
            var results = new List<CMD>();
            int i = 0;
            List<string> SpaceList = new List<string>();
            string SpaceChar = "";
            while (i < input.Length)
            {
                if (input[i] == '\\' && input.Substring(i + 1).StartsWith(CMDName))
                {
                    i += CMDName.Length + 1;
                    var parameters = new List<string>();
                    while (i < input.Length && (input[i] == '{' || char.IsWhiteSpace(input[i])))
                    {
                        while (i < input.Length && char.IsWhiteSpace(input[i]))
                        {
                            i++;
                            SpaceChar += input[i];
                        }
                        if (i < input.Length && input[i] == '{')
                        {
                            int braceLevel = 1;
                            int paramStart = ++i;
                            var paramBuilder = new StringBuilder();
                            while (i < input.Length && braceLevel > 0)
                            {
                                if (input[i] == '{') braceLevel++;
                                else if (input[i] == '}') braceLevel--;
                                if (braceLevel > 0)
                                {
                                    paramBuilder.Append(input[i]);
                                }
                                i++;
                            }
                            if (paramStart < input.Length && i > paramStart)
                            {
                                parameters.Add(paramBuilder.ToString());
                                SpaceList.Add(SpaceChar);
                                SpaceChar = "";
                            }
                        }
                    }
                    string FullCMD = $"\\{CMDName}{{{string.Join("}{", parameters)}}}";
                    results.Add(new CMD() { Name = CMDName, Parameter = parameters, All=FullCMD });
                }
                else
                {
                    i++;
                }
            }
            return results;
        }
    }
}
