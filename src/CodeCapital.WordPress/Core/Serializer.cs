using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeCapital.WordPress.Core
{
    /// <summary>
    /// Serializer taken from internet, needs to be fixed - bad code
    /// https://www.php.net/manual/en/function.serialize.php
    /// https://www.php.net/manual/en/function.unserialize.php
    /// https://github.com/steelbrain/php-serialize/blob/master/src/unserialize.js
    /// https://github.com/naholyr/js-php-unserialize/blob/master/php-unserialize.js
    /// https://github.com/kisli/pherialize/blob/master/pherialize/unserialize.cpp
    /// https://github.com/bd808/php-unserialize-js/blob/master/phpUnserialize.js
    /// https://github.com/kayahr/pherialize/blob/master/src/main/java/de/ailis/pherialize/Unserializer.java
    /// </summary>
    public class Serializer
    {
        //types:
        // N = null
        // b = bool
        // s = string
        // i = int
        // d = double
        // O = object (not implemented)
        // a = array (hashtable)

        private Dictionary<Hashtable, bool> _seenHashTables; //for serialize (to infinte prevent loops)
        private Dictionary<ArrayList, bool> _seenArrayLists; //for serialize (to infinte prevent loops) lol

        private int _position; //for unserialize

        public bool XMLSafe = true; //This member tells the serializer wether or not to strip carriage returns from strings when serializing and adding them back in when deserializing
                                    //http://www.w3.org/TR/REC-xml/#sec-line-ends

        public Encoding StringEncoding = new UTF8Encoding();

        private System.Globalization.NumberFormatInfo nfi;

        public Serializer()
        {
            nfi = new System.Globalization.NumberFormatInfo
            {
                NumberGroupSeparator = "",
                NumberDecimalSeparator = "."
            };
        }

        public string Serialize(object obj)
        {
            _seenArrayLists = new Dictionary<ArrayList, bool>();
            _seenHashTables = new Dictionary<Hashtable, bool>();

            return serialize(obj, new StringBuilder()).ToString();
        }//Serialize(object obj)

        private StringBuilder serialize(object obj, StringBuilder sb)
        {
            if (obj == null) return sb.Append("N;");

            if (obj is string)
            {
                string str = (string)obj;
                if (XMLSafe)
                {
                    str = str.Replace("\r\n", "\n");//replace \r\n with \n
                    str = str.Replace("\r", "\n");//replace \r not followed by \n with a single \n  Should we do this?
                }
                return sb.Append("s:" + StringEncoding.GetByteCount(str) + ":\"" + str + "\";");
            }
            if (obj is bool) return sb.Append("b:" + (((bool)obj) ? "1" : "0") + ";");

            if (obj is int)
            {
                int i = (int)obj;
                return sb.Append("i:" + i.ToString(nfi) + ";");
            }
            if (obj is long)
            {
                long i = (long)obj;
                return sb.Append("i:" + i.ToString(nfi) + ";");
            }
            if (obj is double)
            {
                double d = (double)obj;

                return sb.Append("d:" + d.ToString(nfi) + ";");
            }
            if (obj is ArrayList)
            {
                if (_seenArrayLists.ContainsKey((ArrayList)obj))
                    return sb.Append("N;");//cycle detected

                _seenArrayLists.Add((ArrayList)obj, true);

                ArrayList a = (ArrayList)obj;
                sb.Append("a:" + a.Count + ":{");
                for (int i = 0; i < a.Count; i++)
                {
                    serialize(i, sb);
                    serialize(a[i], sb);
                }
                sb.Append("}");
                return sb;
            }
            if (obj is Hashtable)
            {
                if (_seenHashTables.ContainsKey((Hashtable)obj))
                    return sb.Append("N;");//cycle detected

                _seenHashTables.Add((Hashtable)obj, true);

                Hashtable a = (Hashtable)obj;
                sb.Append("a:" + a.Count + ":{");
                foreach (DictionaryEntry entry in a)
                {
                    serialize(entry.Key, sb);
                    serialize(entry.Value, sb);
                }
                sb.Append("}");
                return sb;
            }

            return sb;

        }//Serialize(object obj)

        public object? Deserialize(string str)
        {
            _position = 0;
            return DeserializeFromString(str);
        }//Deserialize(string str)

        private object? DeserializeFromString(string str)
        {
            if (str == null || str.Length <= _position)
                return new object();

            int start, end, length;
            string stLen;
            switch (str[_position])
            {
                case 'N':
                    _position += 2;
                    return null;
                case 'b': // Stores 1 or 0
                    char chBool;
                    chBool = str[_position + 2];
                    _position += 4;
                    return chBool == '1';
                case 'i':
                    string stInt;
                    start = str.IndexOf(":", _position) + 1;
                    end = str.IndexOf(";", start);
                    stInt = str.Substring(start, end - start);
                    _position += 3 + stInt.Length;
                    object oRet = null;
                    try
                    {
                        //firt try to parse as int
                        oRet = int.Parse(stInt, nfi);
                    }
                    catch
                    {
                        //if it failed, maybe it was too large, parse as long
                        oRet = long.Parse(stInt, nfi);
                    }
                    return oRet;
                case 'd':
                    string stDouble;
                    start = str.IndexOf(":", _position) + 1;
                    end = str.IndexOf(";", start);
                    stDouble = str.Substring(start, end - start);
                    _position += 3 + stDouble.Length;
                    return double.Parse(stDouble, nfi);
                case 's':
                    start = str.IndexOf(":", _position) + 1;
                    end = str.IndexOf(":", start);
                    stLen = str.Substring(start, end - start);
                    int bytelen = int.Parse(stLen);
                    length = bytelen;
                    //This is the byte length, not the character length - so we might  
                    //need to shorten it before usage. This also implies bounds checking
                    if ((end + 2 + length) >= str.Length) length = str.Length - 2 - end;
                    string stRet = str.Substring(end + 2, length);
                    while (StringEncoding.GetByteCount(stRet) > bytelen)
                    {
                        length--;
                        stRet = str.Substring(end + 2, length);
                    }
                    _position += 6 + stLen.Length + length;
                    if (XMLSafe)
                    {
                        stRet = stRet.Replace("\n", "\r\n");
                    }
                    return stRet;
                case 'a':
                    //if keys are ints 0 through N, returns an ArrayList, else returns Hashtable
                    start = str.IndexOf(":", _position) + 1;
                    end = str.IndexOf(":", start);
                    stLen = str.Substring(start, end - start);
                    length = int.Parse(stLen);
                    Hashtable htRet = new Hashtable(length);
                    ArrayList alRet = new ArrayList(length);
                    _position += 4 + stLen.Length; //a:Len:{
                    for (int i = 0; i < length; i++)
                    {
                        //read key
                        object key = DeserializeFromString(str);
                        //read value
                        object val = DeserializeFromString(str);

                        if (alRet != null)
                        {
                            if (key is int && (int)key == alRet.Count)
                                alRet.Add(val);
                            else
                                alRet = null;
                        }
                        htRet[key] = val;
                    }
                    _position++; //skip the }
                    if (_position < str.Length && str[_position] == ';')//skipping our old extra array semi-colon bug (er... php's weirdness)
                        _position++;
                    if (alRet != null)
                        return alRet;
                    else
                        return htRet;
                default:
                    return "";
            }//switch
        }//unserialzie(object)	
    }
}
