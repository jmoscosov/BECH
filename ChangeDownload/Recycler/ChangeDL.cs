using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace ChangeScreens
{
    public class ChangeDL
    {
        private const byte CONTINUE = 0;

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static unsafe byte Incoming(byte** message)
        {
            try
            {
                var msg = NDCMessageIn.FromMessagePointer(message);

                if (msg != null)
                {
                    var msgStr = msg.ToString();

                    if (ProcessMessage(ref msgStr))
                    {
                        Logger.Log("Updating message pointer");

                        msg.Data = Encoding.ASCII.GetBytes(msgStr);
                        msg.ToMessagePointer(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"EXCEPTION: {ex.Message}");
            }

            return CONTINUE;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static unsafe byte Outgoing(byte** message)
        {
            try
            {
                var msg = NDCMessageOut.FromMessagePointer(message);

                if (msg != null)
                {
                    var msgStr = msg.ToString();

                    if (ProcessMessage(ref msgStr))
                    {
                        Logger.Log("Updating message pointer");

                        msg.Data = Encoding.ASCII.GetBytes(msgStr);
                        msg.ToMessagePointer(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"EXCEPTION: {ex.Message}");
            }

            return CONTINUE;
        }

        public static bool ProcessMessage(ref string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;

            var arr = message.Split((char)NDCMessage.FS);

            if (arr != null)
            {
                /* Se verifica si el mensaje es Enhanced Configuration Parameters Load
                 * para cambiar el Option 45 habilitando reciclado */
                /* if (arr.Length > 5 && arr[0].StartsWith("3") && arr[3].Equals("1A"))
                 {
                     Logger.Log("Message is Enhanced Configuration Parameters Load");

                     if (ChangeConfigurationParameters(ref arr[5]))
                     {
                         message = ReassembleMessage(arr);

                         return true;
                     }
                 }*/
                /* Se verifica si el mensaje es carga de pantallas para cambiar las pantallas en donde se muestran menu */
                if (arr.Length > 4 && arr[0].StartsWith("3") && arr[3].Equals("11"))
                {
                    /*
                    for (int i = 4; i < arr.Length; i++)
                    {
                        switch (arr[i].Substring(0, 3))
                        {
                            case "014":
                                Logger.Log($" JM185384 - ChangeScreens - Screen number to Parse: {arr[i].Substring(0, 3)}");
                                if (ChangeScreenData(ref arr, i))
                                {
                                    message = ReassembleMessage(arr);
                                    return true;
                                }
                                break;
                            case "030":
                                Logger.Log($" JM185384 - ChangeScreens - Screen number to Parse: {arr[i].Substring(0, 3)}");
                                if (ChangeScreenData(ref arr, i))
                                {
                                    message = ReassembleMessage(arr);
                                    return true;
                                }
                                break;
                            default:
                                break;
                        }

                    }
                    */
                }
                // Verifica si es un mensaje de configuracion de statedata
                  if (arr.Length > 4 && arr[0].StartsWith("3") && arr[3].Equals("12"))
                   {
                    //int index = 4;
                    for (int index = 4; index < arr.Length; index++)
                    {
                       Logger.Log($"JM185384 - Message is State Table Load: {arr[index]}");
                        switch (arr[index].Substring(0, 3))
                        {
                            case "910":
                                Logger.Log($"JM185384 - ChangeStateData - State number to Parse: {arr[index].Substring(0, 3)}");
                                if (ChangeStateData(ref arr, index, "910Y910138138912911005012000"))
                                {
                                    message = ReassembleMessage(arr);

                                    return true;
                                }
                                break;
                            case "060":
                                Logger.Log($"JM185384 - ChangeStateData - State number to Parse: {arr[index].Substring(0, 3)}");
                                if (ChangeStateData(ref arr, index, "060Y060138138062061005012000"))
                                {
                                    message = ReassembleMessage(arr);

                                    return true;
                                }
                                break;
                            case "401":
                                Logger.Log($"JM185384 - ChangeStateData - State number to Parse: {arr[index].Substring(0, 3)}");
                                if (ChangeStateData(ref arr, index, "401Y060138138403061005012000"))
                                {
                                    message = ReassembleMessage(arr);

                                    return true;
                                }
                                break;
                            case "402":
                                Logger.Log($"JM185384 - ChangeStateData - State number to Parse: {arr[index].Substring(0, 3)}");
                                if (ChangeStateData(ref arr, index, "402F026141138401402255012027"))
                                {
                                    message = ReassembleMessage(arr);

                                    return true;
                                }
                                break;
                            default:
                                break;
                        }

                    }
                   }

            }

            return false;
        }

        private static string ReassembleMessage(string[] arr)
        {
            Logger.Log("Re-assembling message for AANDC");

            var sb = new StringBuilder();

            for (int i = 0; i < arr.Length; i++)
            {
                if (i > 0)
                    sb.Append((char)NDCMessage.FS);

                sb.Append(arr[i]);
            }

            var message = sb.ToString();

            Logger.Log($"Message was modified: {message}");

            return message;
        }

        private static bool ChangeScreenData(ref string[] arr, int index)
        {
            const char FS = '\x1C';
            const char GS = '\x1D';
            const char FF = '\x0C';
            const char SI = '\x0F';
            const char ESC = '\x1B';
            const char backslash = '\x5C';

            var modified = false;
            var sb = new StringBuilder();
            byte[] Data = new byte[15];
            Logger.Log($" JM185384 - ChangeScreens - Original Screen data : {arr[index]}");
            if (arr[index].StartsWith("014"))
                sb.Append("014" + FF + SI + "@@" + ESC + "P2013" + ESC + backslash + ESC + "[27m" + ESC + "[80m" + SI + "AEPOR FAVOR SELECCIONE" + SI + "BJEL PRODUCTO" + SI + "CECON QUE DESEA OPERAR" + SI + "E@CUENTA" + SI + "E1CUENTA" + SI + "F@CORRIENTE" + SI + "H@LINEA" + SI + "I@DE CREDITO" + SI + "K@CUENTA DE" + SI + "I1CREDITO" + SI + "L@AHORRO" + SI + "E7 VISTA /" + SI + "F1CUENTA RUT");
          /*  if (arr[index].StartsWith("017"))
                Data = Encoding.ASCII.GetBytes("017" + FF + SI + "@@" + ESC + "P2013" + ESC + backslash + ESC + "[27m" + ESC + "[80m" + SI + "@HTARJETA DE CREDITO" + SI + "CBOPERACION QUE DESEA REALIZAR" + SI + "HA AVANCE EN" + SI + "H1  CONSULTA" + SI + "IA EFECTIVO" + SI + "I1  DE SALDO" + SI + "KA CAMBIO NUMERO" + SI + "LA SECRETO" + SI + "N1  SALIR" + FS);*/
            if (arr[index].StartsWith("030"))
                sb.Append("030" + FF + SI + "@@" + ESC + "P2013" + ESC + backslash + ESC + "[27m" + ESC + "[80m" + SI + "@GCUENTA CORRIENTE" + SI + "BDPOR FAVOR SELECCIONE LA" + SI + "CBOPERACION QUE DESEA REALIZAR" + SI + "EAGIRO RAPIDO" + SI + "FAPOR $10.000" + SI + "HAGIRO RAPIDO" + SI + "IAPOR $20.000" + SI + "KAGIRO RAPIDO" + SI + "LAPOR $50.000" + SI + "NAGIRO POR" + SI + "OAOTRO MONTO");

            //Data = Encoding.ASCII.GetBytes("030" + FF + SI + "@@" + ESC + "P2013" + ESC + backslash + ESC + "[27m" + ESC + "[80m" + SI + "@GCUENTA CORRIENTE" + SI + "BDPOR FAVOR SELECCIONE LA" + SI + "CBOPERACION QUE DESEA REALIZAR" + SI + "EA  GIRO RAPIDO" + SI + "F@<< POR $ 5.000" + SI + "HA  GIRO RAPIDO" + SI + "I@<< POR $15.000" + SI + "KA  GIRO RAPIDO" + SI + "L@<< POR $50.000" + SI + "NA  GIRO POR" + SI + "O@<< OTRO MONTO" + FS);

            // sb.Append(Encoding.Default.GetString(Data));
            Logger.Log($" JM185384 - ChangeScreens - New Screen data : {sb.ToString()}");
            // string str = Convert.ToBase64String(Data);
            arr[index] = sb.ToString();//Encoding.Default.GetString(Data);
            modified = true;


            return modified;
        }
        private static bool ChangeStateData(ref string[] arr, int index, string newValue)
        {
            var sb = new StringBuilder();
            Logger.Log($" JM185384 - ChangeState - Original State data : {arr[index]}");

            sb.Append(newValue);
            Logger.Log($" JM185384 - ChangeState - New State data : {sb.ToString()}");
            arr[index] = sb.ToString();
            
            return true;
        }

        private static bool ChangeConfigurationParameters(ref string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            var sb = new StringBuilder();
            var current = string.Empty;
            var option = string.Empty;
            var modified = false;
            var optionsFound = new List<string>();

            for (int i = 0; i < s.Length; i++)
            {
                Logger.Log($"[ChangeConfigurationParameters -> S value ] -> {s[i]}");
                current += s[i];
                Logger.Log($"[ChangeConfigurationParameters -> Current Length ] -> {current.Length}");
                if (current.Length == 2 && string.IsNullOrEmpty(option))
                {
                    sb.Append(current);
                    optionsFound.Add(current);
                    Logger.Log($"[ChangeConfigurationParameters -> current Value ] -> {current}");
                    option = current;
                    Logger.Log($"[ChangeConfigurationParameters -> option Value ] -> {option}");
                    current = string.Empty;
                }
                else if (current.Length == 3 && !string.IsNullOrEmpty(option))
                {
                    if (Config.ConfigurationParameters.ContainsKey(option))
                    {
                        if (int.TryParse(Config.ConfigurationParameters[option].InnerText, out int optionValue))
                        {
                            var maskAttr = Config.ConfigurationParameters[option].Attributes["Mask"];

                            if (maskAttr != null)
                            {
                                if (bool.TryParse(maskAttr.Value, out bool isMask) && isMask)
                                {
                                    if (int.TryParse(current, out int value))
                                    {
                                        value |= optionValue;
                                        Logger.Log($"[ChangeConfigurationParameters ->  Value ] -> {value}");
                                        sb.Append(value.ToString("D3"));
                                        Logger.Log($"[ChangeConfigurationParameters ->  sb value ] -> {sb}");
                                    }
                                }
                                else
                                {
                                    sb.Append(optionValue.ToString("D3"));
                                }
                            }
                            else
                            {
                                sb.Append(optionValue.ToString("D3"));
                            }

                            modified |= true;
                        }
                        else
                        {
                            sb.Append(current);
                        }
                    }
                    else
                    {
                        sb.Append(current);
                    }

                    option = string.Empty;
                    current = string.Empty;
                }
            }

            foreach (var param in Config.ConfigurationParameters)
            {
                if (!optionsFound.Contains(param.Key))
                {
                    sb.Append(param.Key);

                    if (int.TryParse(param.Value.InnerText, out int value))
                        sb.Append(value.ToString("D3"));
                    else
                        sb.Append(param.Value.InnerText);

                    modified |= true;
                }
            }

            if (modified)
            {
                s = sb.ToString();
                Logger.Log($"[ChangeConfigurationParameters ->  s Modified ] -> {s}");
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool ChangeNoteTypes(ref string[] arr)
        {
            if (arr == null)
                return false;

            var index = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                if (!string.IsNullOrEmpty(arr[i]) && arr[i].StartsWith("w"))
                {
                    index = i;

                    break;
                }
            }

            var modified = false;

            if (index > 0)
            {
                var sb = new StringBuilder();
                var current = string.Empty;
                var noteTypeId = true;

                sb.Append("w");

                for (int i = 1; i < arr[index].Length; i++)
                {
                    current += arr[index][i];

                    if (current.Length == 2 && noteTypeId)
                    {
                        if (Config.NoteMappings.ContainsKey(current))
                        {
                            sb.Append(Config.NoteMappings[current].InnerText);

                            modified |= true;
                        }
                        else
                        {
                            sb.Append(current);
                        }

                        current = string.Empty;
                        noteTypeId = false;
                    }
                    else if (current.Length == Config.NoteCountLength && !noteTypeId)
                    {
                        sb.Append(current);

                        current = string.Empty;
                        noteTypeId = true;
                    }
                }

                if (modified)
                    arr[index] = sb.ToString();
            }

            return modified;
        }
        private static bool ChangeNoteTypesDeposit(ref string[] arr)
        {
            string intNnotesbyType = "";
            if (arr == null)
                return false;

            var index = 0;
            Logger.Log($"[ChangeNoteTypesDeposit ->  Message Entry ] -> {arr}");
            for (int i = 0; i < arr.Length; i++)
            {
                if (!string.IsNullOrEmpty(arr[i]) && arr[i].StartsWith("w"))
                {
                    index = i;

                    break;
                }
            }

            var modified = false;

            if (index > 0)
            {
                Logger.Log($"[ChangeNoteTypesDeposit ->  Old Parameters ] -> {arr[index]}");
                var sb = new StringBuilder();
                var current = string.Empty;
                var noteTypeId = true;
                var currentNnotes = string.Empty;
                int intValor1 = 0;

                sb.Append("w");

                for (int i = 1; i < arr[index].Length; i++)
                {

                    current += arr[index][i];

                    if (current.Length == 2)
                    {
                        intNnotesbyType += arr[index][i + 1];
                        intNnotesbyType += arr[index][i + 2];
                        if ((i + 3) < arr[index].Length)
                        {
                            intNnotesbyType += arr[index][i + 3];
                            i = i + 3;
                        }
                        else
                        {
                            i = i + 2;
                        }
                        // intNnotesbyType += arr[index][i + 3];

                        intValor1 = Convert.ToInt32(intNnotesbyType);
                        bool bExit = false;
                        int valorActual = intValor1;
                        int valorMod = 0;
                        if (valorActual >= 100)
                        {
                            do
                            {
                                if (valorActual >= 200)
                                {
                                    valorMod = valorActual - 99;
                                }
                                else if (valorMod >= 300)
                                {

                                }
                                else
                                {
                                    valorMod = valorActual % 99;
                                }

                                int valorFinal = 0;
                                valorFinal = valorActual - valorMod;

                                sb.Append(current + valorFinal.ToString("D2"));
                                if (valorMod < 99)
                                {
                                    sb.Append(current + valorMod.ToString("D2"));
                                    bExit = false;

                                }
                                else
                                {
                                    valorActual = valorMod;
                                    bExit = true;
                                }
                            } while (bExit);
                            //}
                        }
                        else
                        {
                            sb.Append(current + intValor1.ToString("D2"));
                        }
                        modified |= true;

                        current = string.Empty;
                        intNnotesbyType = string.Empty;
                    }
                    //current = string.Empty;
                }

                if (modified)
                    arr[index] = sb.ToString();
            }
            Logger.Log($"[ChangeNoteTypesDeposit ->  new Parameters ] -> {arr[index]}");
            return modified;
        }
        //private static bool ChangeLastTxStatusMaxLength(ref string[] arr)
        //{
        //    if (arr?.Length > 13 && !string.IsNullOrEmpty(arr[13]) && arr[13].StartsWith("2"))
        //    {
        //        Logger.Log($"Last transaction status information: {arr[13]}");

        //        if (arr[13].Length > Config.LastTxStatusMaxLength)
        //        {
        //            Logger.Log("Last transaction status is greater than max length");

        //            arr[13] = arr[13].Substring(0, Config.LastTxStatusMaxLength);

        //            return true;
        //        }
        //    }

        //    return false;
        //}

        private static bool ChangeFitnessData(ref string[] arr)
        {
            if (string.IsNullOrEmpty(arr?[4]))
                return false;

            return AddOrRemoveDevices(ref arr[4], "JA", "w");
        }

        private static bool ChangeHardwareConfiguration(ref string[] arr)
        {
            if (string.IsNullOrEmpty(arr?[6]))
                return false;

            return AddOrRemoveDevices(ref arr[6], "HA", "w");
        }

        private static bool ChangeSuppliesData(ref string[] arr)
        {
            if (string.IsNullOrEmpty(arr?[4]))
                return false;
            Logger.Log($"[ChangeSuppliesData] -> calling to AddOrRemoveDevices {arr[4]}");
            return AddOrRemoveDevices(ref arr[4], "IA", "w");
        }

        private static bool ChangeUnsolicitedFitnessData(ref string[] arr)
        {
            if (string.IsNullOrEmpty(arr?[4]))
                return false;

            // CHANGE TEMPLATE DENOMINATION ORDER
            string digStatus = arr[3].Substring(0, 2);
            string cassetteData = arr[3].Substring(2, 150);
            string cassetteDetail1 = ChangeTemplateDenominationOrder(cassetteData.Substring(0, 50));
            string cassetteDetail2 = ChangeTemplateDenominationOrder(cassetteData.Substring(50, 50));
            string cassetteDetail3 = ChangeTemplateDenominationOrder(cassetteData.Substring(100, 50));
            string totalNotesCount = arr[3].Substring(152);

            arr[3] = digStatus + cassetteDetail1 + cassetteDetail2 + cassetteDetail3 + totalNotesCount;

            // CHANGE FITNESS
            if (arr[4].Length > 1)
                arr[4] = "0";

            if (arr[5].Length > 2)
                arr[5] = "00";

            if (arr[6].Length > 1)
                arr[6] = "0";

            return true;
        }

        private static string ChangeTemplateDenominationOrder(string denominations)
        {
            StringBuilder newDetail = new StringBuilder();
            Logger.Log($"[ChangeTemplateDenominationOrder] start");
            newDetail.Append(' ', 9);

            char[] denomDetail = denominations.Substring(0, 5).ToCharArray();

            newDetail.Append(denomDetail[1]);
            newDetail.Append(denomDetail[2]);
            newDetail.Append(denomDetail[3]);
            newDetail.Append(denomDetail[0]);
            newDetail.Append(denomDetail[4]);
            Logger.Log($"[ChangeTemplateDenominationOrder] ->denomDetail1 {denomDetail[1]}");
            Logger.Log($"[ChangeTemplateDenominationOrder] ->denomDetail2 {denomDetail[2]}");
            Logger.Log($"[ChangeTemplateDenominationOrder] ->denomDetail3 {denomDetail[3]}");
            Logger.Log($"[ChangeTemplateDenominationOrder] ->denomDetail4 {denomDetail[0]}");
            Logger.Log($"[ChangeTemplateDenominationOrder] ->denomDetail5 {denomDetail[4]}");
            newDetail.Append('~', 36);
            Logger.Log($"[ChangeTemplateDenominationOrder] -> newDetail {newDetail}");
            Logger.Log($"[ChangeTemplateDenominationOrder] Exit");
            return newDetail.ToString();
        }

        private static bool AddOrRemoveDevices(ref string devices, string messageId, string prefix = "")
        {

            if (string.IsNullOrEmpty(devices))
                return false;

            //var devicesData = devices.Substring(prefix.Length);
            var devicesData = devices;

            Logger.Log($"Adding/removing devices for {messageId}: {devicesData}");

            var devsArr = devicesData.Split((char)NDCMessage.GS);

            if (devsArr.Length > 0)
            {
                var dict = new Dictionary<string, string>();

                // Parse device data
                foreach (var s in devsArr)
                {
                    Logger.Log($"Current device AddOrRemoveDevices {s}");
                    if (s.Length >= 2)
                    {
                        var dig = s.Substring(0, 1);
                        var data = s.Substring(1);

                        if (prefix == "w" && (messageId == "IA" || messageId == "JA"))
                        {
                            Logger.Log($"Current device data {dig} : {data}");
                            if (dig == "w" && (data.Length > 1))
                            {
                                data = data.Substring(0, 1);
                                Logger.Log($"Current device data {dig} : {data}");
                            }

                        }

                        dict.Add(dig, data);
                    }
                    else
                    {
                        Logger.Log($"Not enought information to obtain DIG and data: {s}");
                    }
                }

                // Add or remove devices according to configuration
                foreach (var f in GetDevices(messageId))
                {
                    if (string.IsNullOrEmpty(f.Value.InnerText))
                    {
                        Logger.Log($"Removing device {f.Key}");

                        if (dict.ContainsKey(f.Key))
                            dict.Remove(f.Key);
                    }
                    else
                    {
                        Logger.Log($"Adding device {f.Key}");

                        if (dict.ContainsKey(f.Key))
                            dict[f.Key] = GetDataValue(f.Value.InnerText, dict[f.Key]);
                        else
                            dict.Add(f.Key, GetDataValue(f.Value.InnerText));
                    }
                }

                // Re-assemble devices data
                if (dict.Count > 0)
                {
                    Logger.Log("Re-assembling devices data");

                    var sb = new StringBuilder();
                    int i = 0;

                    //sb.Append(prefix);

                    foreach (var dev in dict)
                    {
                        if (i > 0)
                            sb.Append((char)NDCMessage.GS);

                        sb.Append(dev.Key);
                        sb.Append(dev.Value);

                        i++;
                    }

                    devices = sb.ToString();
                    Logger.Log($" Current Devices return : {devices}");
                    return true;
                }
            }

            return false;
        }

        private static IDictionary<string, XmlNode> GetDevices(string messageId)
        {
            var dict = new Dictionary<string, XmlNode>();

            foreach (var node in Config.Devices)
            {
                if (node.Key == messageId)
                {
                    foreach (XmlNode subNode in node.Value?.ChildNodes)
                    {
                        var dig = subNode.Attributes?["DIG"]?.Value;

                        if (!string.IsNullOrEmpty(dig))
                            dict.Add(dig, subNode);
                    }
                }
            }

            return dict;
        }

        private static string GetDataValue(string newValue, string currentValue = "")
        {
            if (string.IsNullOrEmpty(newValue))
                return string.Empty;

            if (string.IsNullOrEmpty(currentValue))
                return newValue.Replace("?", "");

            string data = null;
            string mask = null;

            if (newValue.Length < currentValue.Length)
            {
                data = currentValue;
                mask = newValue + new string('\0', currentValue.Length - newValue.Length);
            }
            else if (currentValue.Length < newValue.Length)
            {
                data = currentValue + new string('\0', newValue.Length - currentValue.Length);
                mask = newValue;
            }
            else
            {
                data = currentValue;
                mask = newValue;
            }

            if (!string.IsNullOrEmpty(newValue) && !string.IsNullOrEmpty(currentValue))
            {
                var sb = new StringBuilder();

                for (int i = 0; i < mask.Length; i++)
                {
                    if (mask[i] == '?')
                    {
                        if (data[i] != '\0')
                            sb.Append(data[i]);
                    }
                    else
                    {
                        if (mask[i] != '\0')
                            sb.Append(mask[i]);
                    }
                }

                return sb.ToString();
            }

            return newValue.Replace("?", "");
        }
    }
}
