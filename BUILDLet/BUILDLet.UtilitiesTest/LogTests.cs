using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using BUILDLet.Utilities;

namespace BUILDLet.Utilities.Tests
{
    [TestClass()]
    public class LogTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Log_WriteLine_ExceptionTest()
        {
            new Log(methodBracket: new char[] { '1', '2', '3' });
        }

        [TestMethod()]
        public void Log_MethodNameTest_withTitle()
        {
            new Log(title: true);
        }

        [TestMethod()]
        public void Log_WriteTitleTest()
        {
            new Log(methodBracket: new char[] { '!', '|' }).WriteTitle();
            new Log(methodBracket: new char[] { '★', '☆' }).WriteTitle();
        }

        [TestMethod()]
        public void Log_LogOutputTest()
        {
            new Log().WriteLine("Standard Output Stream (Default)");

            new Log(stream: LogOutputStream.StandardError).WriteLine("Standard Error Output Stream");

            new Log(stream: LogOutputStream.Trace).WriteLine("Trace Listner");
        }

        [TestMethod()]
        public void Log_WriteLineTest()
        {
            Log log = null;
            bool?[] comb = null;

            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        // methodName:true, timeStamp:false (Default)
                        log = new Log();
                        log.WriteLine("new Log()");
                        break;

                    case 1:
                        // methodName:true, timeStamp:true
                        log = new Log(timeStamp: true);
                        log.WriteLine("new Log(timeStamp:true)");
                        break;

                    case 2:
                        // methodName:false, timeStamp:false
                        log = new Log(methodName: false);
                        log.WriteLine("new Log(methodName:false)");
                        break;

                    case 3:
                        // methodName:false, timeStamp:true
                        log = new Log(methodName: false, timeStamp: true);
                        log.WriteLine("new Log(methodName:false, timeStamp:true)");
                        break;

                    default:
                        break;
                }

                for (int j = 0; j < 3 * 3; j++)
                {
                    switch (j)
                    {
                        case 0:
                            comb = new bool?[] { null, null };
                            break;

                        case 1:
                            comb = new bool?[] { null, true };
                            break;

                        case 2:
                            comb = new bool?[] { null, false };
                            break;

                        case 3:
                            comb = new bool?[] { true, null };
                            break;

                        case 4:
                            comb = new bool?[] { true, true };
                            break;

                        case 5:
                            comb = new bool?[] { true, false };
                            break;

                        case 6:
                            comb = new bool?[] { false, false };
                            break;

                        case 7:
                            comb = new bool?[] { false, true };
                            break;

                        case 8:
                            comb = new bool?[] { false, false };
                            break;

                        default:
                            break;
                    }

                    log.WriteLine(string.Format("\"methodName={0}, timeStamp={1}\"", comb[0], comb[1]), comb[0], comb[1]);
                }
            }
        }
    }
}
