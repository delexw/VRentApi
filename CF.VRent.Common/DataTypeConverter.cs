using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Runtime.Serialization;

namespace CF.VRent.Common
{
    public static class DataTypeConverter
    {
        #region Functions
        /// <summary>
        /// object to int
        /// </summary>
        /// <returns></returns>
        public static int ToInt(this object obj)
        {
            if (obj == null)
                return 0;
            if (obj.Equals(DBNull.Value))
                return 0;

            try
            {
                return Convert.ToInt32(obj);
            }
            catch {
                return 0;
            }
        }

        /// <summary>
        /// object to long
        /// </summary>
        /// <returns></returns>
        public static long ToLong(this object obj)
        {
            if (obj == null)
                return 0;
            if (obj.Equals(DBNull.Value))
                return 0;

            try
            {
                return Convert.ToInt64(obj);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// obj to bool
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ToBool(this object obj)
        {
            if (obj == null)
                return false;
            if (obj.Equals(DBNull.Value))
                return false;

            try
            {
                return Convert.ToBoolean(obj);
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// object to int?
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int? ToIntNull(this object obj)
        {
            if (obj == null)
                return null;
            if (obj.Equals(DBNull.Value))
                return null;

            return ToInt(obj);
        }


        /// <summary>
        /// object to string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToStr(this object obj)
        {
            if (obj == null)
                return "";
            if (obj.Equals(DBNull.Value))
                return "";
            return Convert.ToString(obj);
        }


        /// <summary>
        /// object to decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object obj)
        {
            if (obj == null)
                return 0;
            if (obj.Equals(DBNull.Value))
                return 0;

            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// object to decimal?
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal? ToDecimalNull(this object obj)
        {
            if (obj == null)
                return null;
            if (obj.Equals(DBNull.Value))
                return null;

            return ToDecimal(obj);
        }

        /// <summary>
        /// obj to double
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double ToDouble(this object obj)
        {
            if (obj == null)
                return 0;
            if (obj.Equals(DBNull.Value))
                return 0;

            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// object to double?
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double? ToDoubleNull(this object obj)
        {
            if (obj == null)
                return null;
            if (obj.Equals(DBNull.Value))
                return null;

            return ToDouble(obj);
        }

        /// <summary>
        /// obj to datetime?
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? ToDateNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// obj to datetime
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime ToDate(this object obj)
        {
            if (obj == null)
            {
                return DateTime.Now;
            }
            return Convert.ToDateTime(obj);
        }

        /// <summary>
        /// obj to datetime with specified format
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static DateTime ToDateWithFormatter(this object obj, string formatter = "yyyyMMddHHmmss")
        {
            if (obj == null)
            {
                return DateTime.MinValue;
            }
            return DateTime.ParseExact(obj.ToStr(), formatter, System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// obj to Guid
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Guid ToGuid(this object obj)
        {
            Guid r;
            if (obj == null)
            {
                return Guid.Empty;
            }
            else if (Guid.TryParse(obj.ToStr(), out r))
            {
                return r;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// obj to Guid?
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Guid? ToGuidNull(this object obj)
        {
            Guid r;
            if (obj == null)
            {
                return null;
            }
            else if (Guid.TryParse(obj.ToStr(), out r))
            {
                return r;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Array<T> To DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(this IEnumerable<T> array)
        {
            var ret = new DataTable();
            ret.TableName = typeof(T).Name.Replace(".", "_");
            foreach (PropertyDescriptor dp in TypeDescriptor.GetProperties(typeof(T)))
            {
                if (dp.Attributes[typeof(DataMemberAttribute)] != null)
                {
                    //deal with nullable type
                    Type t = dp.PropertyType.Name == "Nullable`1" ? dp.PropertyType.GetGenericArguments()[0] : dp.PropertyType;
                    ret.Columns.Add(dp.Name, t);
                }
            }
            foreach (T item in array)
            {
                var Row = ret.NewRow();
                foreach (PropertyDescriptor dp in TypeDescriptor.GetProperties(typeof(T)))
                {
                    if (dp.Attributes[typeof(DataMemberAttribute)] != null)
                    {
                        Row[dp.Name] = dp.GetValue(item) == null ? DBNull.Value : dp.GetValue(item);
                    }
                }
                ret.Rows.Add(Row);
            }
            return ret;
        }

        /// <summary>
        /// Hanzi to pinyin
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string ParseStringToPinyin(this string exp, bool comma = false)
        {
            exp = exp.Trim();
            string pinYin = "", str = null;
            char[] chars = exp.ToCharArray();
            foreach (char c in chars)
            {
                try
                {
                    str = chs2py.convert(c.ToString());
                    if (comma)
                    {
                        pinYin += str.Substring(0, 1).ToUpper() + str.Substring(1) + ", ";
                    }
                    else
                    {
                        pinYin += str.Substring(0, 1) + str.Substring(1);
                    }
                }
                catch
                {
                    pinYin += c;
                }
            }
            return pinYin;
        }
        /// <summary>
        /// Hanzi to pinyin and the first word is Upper
        /// </summary>
        /// <param name="str">Hanzi string</param>
        /// <returns>pinyin</returns>
        public static string ParseStringToPinyinWordFirst(this string str)
        {
            try
            {
                String _Temp = null;
                for (int i = 0; i < str.Length; i++)
                {
                    if (i == 0)
                    {
                        _Temp = _Temp + str.Substring(i, 1).ParseStringToPinyin(true);
                    }
                    else if (i == 1)
                    {
                        var sub = str.Substring(i, 1).ParseStringToPinyin();
                        if (sub != str.Substring(i, 1))
                        {
                            _Temp = _Temp + sub.Substring(0, 1).ToUpper() + sub.Substring(1);
                        }
                        else
                        {
                            _Temp = _Temp + str.Substring(i, 1).ParseStringToPinyin();
                        }
                    }
                    else
                    {
                        _Temp = _Temp + str.Substring(i, 1).ParseStringToPinyin();
                    }
                }
                return _Temp;
            }
            catch
            {
                throw new Exception("@Error：" + str + ",can't convert it to pinyin");
            }
        }

        /// <summary>
        /// Obj to Dictionary(string,string)
        /// </summary>
        /// <param name="o"></param>
        /// <param name="firstWordLower">The first word of Key is lower or upper</param>
        /// <param name="excludedMembers">Members dont want to be added in Dictionary</param>
        /// <returns></returns>
        public static Dictionary<T, V> ToDictionary<T, V>(this Object obj, bool firstWordKeyLower = true, params string[] excludedMembers)
            where T : class
            where V : class
        {
            Dictionary<T, V> map = new Dictionary<T, V>();

            Type t = obj.GetType();

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();

                if (mi != null && mi.IsPublic && !excludedMembers.Contains(p.Name))
                {
                    var tempKey = (firstWordKeyLower ? p.Name.Substring(0, 1).ToLower() + p.Name.Substring(1) : p.Name) as T;
                    var tempValue = mi.Invoke(obj, new Object[] { }) as V;
                    if (tempValue != null)
                    {
                        map.Add(tempKey, tempValue);
                    }
                }
            }

            return map;

        }

        /// <summary>
        /// Obj to Key=Value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="firstWordKeyLower">The first word of Key is lower or upper</param>
        /// <param name="excludedMembers">Members dont want to be added in Dictionary</param>
        /// <returns></returns>
        public static string ToKeyValueString(this Object obj, bool firstWordKeyLower = true, params string[] excludedMembers)
        {
            Type t = obj.GetType();

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            StringBuilder builder = new StringBuilder();

            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();

                if (mi != null && mi.IsPublic && !excludedMembers.Contains(p.Name))
                {
                    var tempKey = (firstWordKeyLower ? p.Name.Substring(0, 1).ToLower() + p.Name.Substring(1) : p.Name).ToString();
                    var tempValue = mi.Invoke(obj, new Object[] { });
                    if (tempValue != null)
                    {
                        builder.Append(tempKey + "=" + tempValue + "&");
                    }
                }
            }
            return builder.ToString().Substring(0, builder.Length - 1);
        }

        /// <summary>
        /// IDictionary to Entity
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <typeparam name="V">Dictionary Value Type</typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static T ToEntity<T, W, V>(this IDictionary<W, V> dic, bool firstWordKeyLower = false)
            where T : class,new()
            where V : class
            where W : class
        {
            T obj = new T();

            PropertyInfo[] tProperty = obj.GetType().GetProperties();

            foreach (W key in dic.Keys)
            {
                var strKey = key.ToStr();
                var keyProperty = tProperty.Where(p => p.Name.ToString() == (!firstWordKeyLower ? strKey.Substring(0, 1).ToUpper() + strKey.Substring(1) : strKey)).FirstOrDefault();
                if (keyProperty != null)
                {
                    keyProperty.SetValue(obj, Convert.ChangeType(dic[key], keyProperty.PropertyType), null);
                }
            }

            return obj;
        }

        /// <summary>
        /// Dictionary(string,string) to Key=Value
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToKeyValueString(this Dictionary<string, string> data)
        {
            SortedDictionary<string, string> treeMap = new SortedDictionary<string, string>(StringComparer.Ordinal);

            foreach (KeyValuePair<string, string> kvp in data)
            {
                treeMap.Add(kvp.Key, kvp.Value);
            }

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> element in treeMap)
            {
                builder.Append(element.Key + "=" + element.Value + "&");
            }

            return builder.ToString().Substring(0, builder.Length - 1);
        }

        /// <summary>
        /// Key=Value to Dictionary(string,string)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(this string data)
        {
            if (null == data || 0 == data.Length)
            {
                return null;
            }
            string[] arrray = data.Split(new char[] { '&' });
            Dictionary<string, string> res = new Dictionary<string, string>();
            foreach (string s in arrray)
            {
                int n = s.IndexOf("=");
                string key = s.Substring(0, n);
                string value = HttpUtility.UrlDecode(s.Substring(n + 1));
                res.Add(key, value);
            }
            return res;
        }

        /// <summary>
        /// encode base64
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static string ToBase64(this string data,Encoding encoder)
        {
            var b = encoder.GetBytes(data);
            return Convert.ToBase64String(b);
        }

        /// <summary>
        /// decode base64
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static string ToBase64Origin(this string data, Encoding encoder)
        {
            var b = Convert.FromBase64String(data);

            return encoder.GetString(b);
        }

        /// <summary>
        /// image base64 string
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToBase64(this Bitmap image, ImageFormat format)
        {
            string imgBaseFormat = "data:image/{0};base64, {1}";
            MemoryStream ms = new MemoryStream();
            image.Save(ms, format);
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();

            return String.Format(imgBaseFormat, format.ToString().ToLower(), Convert.ToBase64String(arr));
        }

        /// <summary>
        /// string to enum
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumString"></param>
        /// <returns></returns>
        public static TEnum ToEnum<TEnum>(this string enumString) where TEnum: struct
        {
            var e = default(TEnum);
            if (Enum.TryParse<TEnum>(enumString, out e))
            {
                return e;
            }
            return e;
        }
        #endregion
    }


    #region Assistant class
    public class chs2py
    {
        private static int[] pyvalue = new int[]{-20319,-20317,-20304,-20295,-20292,-20283,-20265,-20257,-20242,-20230,-20051,-20036,-20032,-20026,   
          -20002,-19990,-19986,-19982,-19976,-19805,-19784,-19775,-19774,-19763,-19756,-19751,-19746,-19741,-19739,-19728,   
          -19725,-19715,-19540,-19531,-19525,-19515,-19500,-19484,-19479,-19467,-19289,-19288,-19281,-19275,-19270,-19263,   
          -19261,-19249,-19243,-19242,-19238,-19235,-19227,-19224,-19218,-19212,-19038,-19023,-19018,-19006,-19003,-18996,   
          -18977,-18961,-18952,-18783,-18774,-18773,-18763,-18756,-18741,-18735,-18731,-18722,-18710,-18697,-18696,-18526,   
          -18518,-18501,-18490,-18478,-18463,-18448,-18447,-18446,-18239,-18237,-18231,-18220,-18211,-18201,-18184,-18183,   
          -18181,-18012,-17997,-17988,-17970,-17964,-17961,-17950,-17947,-17931,-17928,-17922,-17759,-17752,-17733,-17730,   
          -17721,-17703,-17701,-17697,-17692,-17683,-17676,-17496,-17487,-17482,-17468,-17454,-17433,-17427,-17417,-17202,   
          -17185,-16983,-16970,-16942,-16915,-16733,-16708,-16706,-16689,-16664,-16657,-16647,-16474,-16470,-16465,-16459,   
          -16452,-16448,-16433,-16429,-16427,-16423,-16419,-16412,-16407,-16403,-16401,-16393,-16220,-16216,-16212,-16205,   
          -16202,-16187,-16180,-16171,-16169,-16158,-16155,-15959,-15958,-15944,-15933,-15920,-15915,-15903,-15889,-15878,   
          -15707,-15701,-15681,-15667,-15661,-15659,-15652,-15640,-15631,-15625,-15454,-15448,-15436,-15435,-15419,-15416,   
          -15408,-15394,-15385,-15377,-15375,-15369,-15363,-15362,-15183,-15180,-15165,-15158,-15153,-15150,-15149,-15144,   
          -15143,-15141,-15140,-15139,-15128,-15121,-15119,-15117,-15110,-15109,-14941,-14937,-14933,-14930,-14929,-14928,   
          -14926,-14922,-14921,-14914,-14908,-14902,-14894,-14889,-14882,-14873,-14871,-14857,-14678,-14674,-14670,-14668,   
          -14663,-14654,-14645,-14630,-14594,-14429,-14407,-14399,-14384,-14379,-14368,-14355,-14353,-14345,-14170,-14159,   
          -14151,-14149,-14145,-14140,-14137,-14135,-14125,-14123,-14122,-14112,-14109,-14099,-14097,-14094,-14092,-14090,   
          -14087,-14083,-13917,-13914,-13910,-13907,-13906,-13905,-13896,-13894,-13878,-13870,-13859,-13847,-13831,-13658,   
          -13611,-13601,-13406,-13404,-13400,-13398,-13395,-13391,-13387,-13383,-13367,-13359,-13356,-13343,-13340,-13329,   
          -13326,-13318,-13147,-13138,-13120,-13107,-13096,-13095,-13091,-13076,-13068,-13063,-13060,-12888,-12875,-12871,   
          -12860,-12858,-12852,-12849,-12838,-12831,-12829,-12812,-12802,-12607,-12597,-12594,-12585,-12556,-12359,-12346,   
          -12320,-12300,-12120,-12099,-12089,-12074,-12067,-12058,-12039,-11867,-11861,-11847,-11831,-11798,-11781,-11604,   
          -11589,-11536,-11358,-11340,-11339,-11324,-11303,-11097,-11077,-11067,-11055,-11052,-11045,-11041,-11038,-11024,   
          -11020,-11019,-11018,-11014,-10838,-10832,-10815,-10800,-10790,-10780,-10764,-10587,-10544,-10533,-10519,-10331,   
          -10329,-10328,-10322,-10315,-10309,-10307,-10296,-10281,-10274,-10270,-10262,-10260,-10256,-10254,-6475,-6174,-5974};
        private static string[] pystr = new string[]{"a","ai","an","ang","ao","ba","bai","ban","bang","bao","bei","ben","beng","bi","bian","biao",   
        "bie","bin","bing","bo","bu","ca","cai","can","cang","cao","ce","ceng","cha","chai","chan","chang","chao","che","chen",   
        "cheng","chi","chong","chou","chu","chuai","chuan","chuang","chui","chun","chuo","ci","cong","cou","cu","cuan","cui",   
        "cun","cuo","da","dai","dan","dang","dao","de","deng","di","dian","diao","die","ding","diu","dong","dou","du","duan",   
        "dui","dun","duo","e","en","er","fa","fan","fang","fei","fen","feng","fo","fou","fu","ga","gai","gan","gang","gao",   
        "ge","gei","gen","geng","gong","gou","gu","gua","guai","guan","guang","gui","gun","guo","ha","hai","han","hang",   
        "hao","he","hei","hen","heng","hong","hou","hu","hua","huai","huan","huang","hui","hun","huo","ji","jia","jian",   
        "jiang","jiao","jie","jin","jing","jiong","jiu","ju","juan","jue","jun","ka","kai","kan","kang","kao","ke","ken",   
        "keng","kong","kou","ku","kua","kuai","kuan","kuang","kui","kun","kuo","la","lai","lan","lang","lao","le","lei",   
        "leng","li","lia","lian","liang","liao","lie","lin","ling","liu","long","lou","lu","lv","luan","lue","lun","luo",   
        "ma","mai","man","mang","mao","me","mei","men","meng","mi","mian","miao","mie","min","ming","miu","mo","mou","mu",   
        "na","nai","nan","nang","nao","ne","nei","nen","neng","ni","nian","niang","niao","nie","nin","ning","niu","nong",   
        "nu","nv","nuan","nue","nuo","o","ou","pa","pai","pan","pang","pao","pei","pen","peng","pi","pian","piao","pie",   
        "pin","ping","po","pu","qi","qia","qian","qiang","qiao","qie","qin","qing","qiong","qiu","qu","quan","que","qun",   
        "ran","rang","rao","re","ren","reng","ri","rong","rou","ru","ruan","rui","run","ruo","sa","sai","san","sang",   
        "sao","se","sen","seng","sha","shai","shan","shang","shao","she","shen","sheng","shi","shou","shu","shua",   
        "shuai","shuan","shuang","shui","shun","shuo","si","song","sou","su","suan","sui","sun","suo","ta","tai",   
        "tan","tang","tao","te","teng","ti","tian","tiao","tie","ting","tong","tou","tu","tuan","tui","tun","tuo",   
        "wa","wai","wan","wang","wei","wen","weng","wo","wu","xi","xia","xian","xiang","xiao","xie","xin","xing",   
        "xiong","xiu","xu","xuan","xue","xun","ya","yan","yang","yao","ye","yi","yin","ying","yo","yong","you",   
        "yu","yuan","yue","yun","za","zai","zan","zang","zao","ze","zei","zen","zeng","zha","zhai","zhan","zhang",   
        "zhao","zhe","zhen","zheng","zhi","zhong","zhou","zhu","zhua","zhuai","zhuan","zhuang","zhui","zhun","zhuo",   
        "zi","zong","zou","zu","zuan","zui","zun","zuo","xian","wei","jin"};
        public chs2py()
        {
        }
        public static string ConvertWordFirst(string str)
        {
            try
            {
                String _Temp = null;
                for (int i = 0; i < str.Length; i++)
                    _Temp = _Temp + chs2py.convert(str.Substring(i, 1));
                return _Temp;
            }
            catch
            {
                throw new Exception("@错误：" + str + "，不能转换成拼音。");
            }
        }
        public static string convert(string chrstr, string isNullAsVal)
        {
            try
            {
                return convert(chrstr);
            }
            catch
            {
                return isNullAsVal;
            }
        }
        /// <summary>
        /// Get pinyin
        /// </summary>
        /// <param name="chrstr"></param>
        /// <returns></returns>
        public static string convert(string chrstr)
        {
            byte[] array = new byte[2];
            string returnstr = "";
            int chrasc = 0;
            int i1 = 0;
            int i2 = 0;
            char[] nowchar = chrstr.ToCharArray();
            for (int j = 0; j < nowchar.Length; j++)
            {
                switch (nowchar[j].ToString())
                {
                    case ")":
                    case "(":
                    case "_":
                    case "-":
                    case "?":
                    case "）":
                    case "（":
                    case " ":
                        continue;
                    default:
                        break;
                }

                array = System.Text.Encoding.Default.GetBytes(nowchar[j].ToString());
                i1 = (short)(array[0]);
                i2 = (short)(array[1]);

                chrasc = i1 * 256 + i2 - 65536;
                if (chrasc > 0 && chrasc < 160)
                {
                    returnstr += nowchar[j];
                }
                else
                {
                    for (int i = (pyvalue.Length - 1); i >= 0; i--)
                    {
                        if (pyvalue[i] <= chrasc)
                        {
                            returnstr += pystr[i];
                            break;
                        }
                    }
                }
            }
            return returnstr;
        }
    } 
    #endregion
}
