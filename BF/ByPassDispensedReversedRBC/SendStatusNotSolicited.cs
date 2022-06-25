using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Win32;

namespace ByPass
{
    public class SendStatusNotSolicited
    {
        RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\NCR\Advance NDC\ByPassRBC");
        string RBCMessageLetter;
        string sCounters;
        public void send()
        {
            
            Logger.Log($"JM185384 entry to Thread");
            int intTimer03 = NDCMessage.GetIntVal(3129);
            Logger.Log($"Timer03 Value :{intTimer03}");
            bool txReplyMessage = false;
            if (key != null)
            {
                RBCMessageLetter = (string)key.GetValue("ByPassRBCID");
            }
            else
            {
                RBCMessageLetter = "R";
            }
            for (int i = 0; i < intTimer03 ; i++)
            {
                if (NDCMessage.GetIntValUCDI("ByPassUCDI") == 0)
                {
                    txReplyMessage = true;
                    break;
                }
                Thread.Sleep(1000);
            }
            if (!txReplyMessage)
            {

                Logger.Log($"JM185384 Sending Message not solicited with ID : {RBCMessageLetter}");
                NDCMessage.SendStatus(RBCMessageLetter, false, false);
            }
            
            
            Logger.Log($"exit to Thread");
        }

        public void SendCountersToHost()
        {
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(1000);
            }
            int idNotesDispType1 = NDCMessage.GetIntVal(3091);
            int idNotesDispType2 = NDCMessage.GetIntVal(3092);
            int idNotesDispType3 = NDCMessage.GetIntVal(3093);
            int idNotesDispType4 = NDCMessage.GetIntVal(3094);

            int idNotesInType1 = NDCMessage.GetIntVal(3095);
            int idNotesInType2 = NDCMessage.GetIntVal(3096);
            int idNotesInType3 = NDCMessage.GetIntVal(3097);
            int idNotesInType4 = NDCMessage.GetIntVal(3098);

            int idNotesPurgedType1 = NDCMessage.GetIntVal(3099);
            int idNotesPurgedType2 = NDCMessage.GetIntVal(3100);
            int idNotesPurgedType3 = NDCMessage.GetIntVal(3101);
            int idNotesPurgedType4 = NDCMessage.GetIntVal(3102);

            Logger.Log($"JM185384 Notes Dispensed Type 1 :  {idNotesDispType1.ToString("D4")}");
            Logger.Log($"JM185384 Notes Dispensed Type 2 :  {idNotesDispType2.ToString("D4")}");
            Logger.Log($"JM185384 Notes Dispensed Type 3 :  {idNotesDispType3.ToString("D4")}");
            Logger.Log($"JM185384 Notes Dispensed Type 4 :  {idNotesDispType4.ToString("D4")}");

            Logger.Log($"JM185384 Notes Remaining Type 1 :  {idNotesInType1.ToString("D4")}");
            Logger.Log($"JM185384 Notes Remaining Type 2 :  {idNotesInType2.ToString("D4")}");
            Logger.Log($"JM185384 Notes Remaining Type 3 :  {idNotesInType3.ToString("D4")}");
            Logger.Log($"JM185384 Notes Remaining Type 4 :  {idNotesInType4.ToString("D4")}");

            Logger.Log($"JM185384 Notes Rejected Type 1 :  {idNotesPurgedType1.ToString("D4")}");
            Logger.Log($"JM185384 Notes Rejected Type 2 :  {idNotesPurgedType2.ToString("D4")}");
            Logger.Log($"JM185384 Notes Rejected Type 3 :  {idNotesPurgedType3.ToString("D4")}");
            Logger.Log($"JM185384 Notes Rejected Type 4 :  {idNotesPurgedType4.ToString("D4")}");
            sCounters = idNotesDispType1.ToString("D4") + idNotesDispType2.ToString("D4") + idNotesDispType3.ToString("D4") + idNotesDispType4.ToString("D4") + idNotesInType1.ToString("D4") + idNotesInType2.ToString("D4") + idNotesInType3.ToString("D4") + idNotesInType4.ToString("D4") + idNotesPurgedType1.ToString("D4") + idNotesPurgedType2.ToString("D4") + idNotesPurgedType3.ToString("D4") + idNotesPurgedType4.ToString("D4");



            Logger.Log($"JM185384 Sending counters to Host : {sCounters}");
            NDCMessage.SendUnformattedData(sCounters, true);


            Logger.Log($"exit to Thread");
        }
    }
}
