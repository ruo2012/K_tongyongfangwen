﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using 通用访问.DTO;
using 通用访问.自定义序列化;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Diagnostics;

namespace 通用访问
{
    public static class HJSON
    {
        private static List<JsonConverter> _默认自定义序列化 = new List<JsonConverter>();

        static HJSON()
        {
            _默认自定义序列化.Add(new 时间Converter());
            _默认自定义序列化.Add(new IPEndPointConverter());
            _默认自定义序列化.Add(new IPAddressConverter());
            _默认自定义序列化.Add(new EnumConverter<E角色>());
            _默认自定义序列化.Add(new EnumConverter<E数据结构>());
            _默认自定义序列化.Add(new M属性值查询结果Converter());
            _默认自定义序列化.Add(new M方法执行结果Converter());
            _默认自定义序列化.Add(new M实参Converter());
        }

        public static string 序列化(object obj, params JsonConverter[] __自定义序列化)
        {
            return 序列化(obj, true, __自定义序列化);
        }

        public static string 序列化(object obj, bool __格式化, params JsonConverter[] __自定义序列化)
        {
            var settings = new JsonSerializerSettings { Formatting = __格式化 ?  Formatting.Indented : Formatting.None };
            if (__自定义序列化 != null)
            {
                for (int i = 0; i < __自定义序列化.Length; i++)
                {
                    settings.Converters.Add(__自定义序列化[i]);
                }
            }
            _默认自定义序列化.ForEach(q => settings.Converters.Add(q));
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T 反序列化<T>(string 字符串, params JsonConverter[] __自定义序列化)
        {
            //return new JavaScriptSerializer().Deserialize<T>(字符串); //H序列化.FromJSON字符串
            var settings = new JsonSerializerSettings { };
            if (__自定义序列化 != null)
            {
                for (int i = 0; i < __自定义序列化.Length; i++)
                {
                    settings.Converters.Add(__自定义序列化[i]);
                }
                _默认自定义序列化.ForEach(q => settings.Converters.Add(q));
                return JsonConvert.DeserializeObject<T>(字符串, settings);
            }
            _默认自定义序列化.ForEach(q => settings.Converters.Add(q));
            return JsonConvert.DeserializeObject<T>(字符串);
        }

        public static object 反序列化(Type 类型, string 字符串, params JsonConverter[] __自定义序列化)
        {
            var settings = new JsonSerializerSettings {  };
            if (__自定义序列化 != null)
            {
                for (int i = 0; i < __自定义序列化.Length; i++)
                {
                    settings.Converters.Add(__自定义序列化[i]);
                }
                _默认自定义序列化.ForEach(q => settings.Converters.Add(q));
                return JsonConvert.DeserializeObject(字符串, 类型, settings);
            }
            _默认自定义序列化.ForEach(q => settings.Converters.Add(q));
            return JsonConvert.DeserializeObject(字符串, 类型);
        }

        public static E数据结构 识别数据结构(string __字符串)
        {
            if (__字符串 == null)
            {
                return E数据结构.单值;
            }
            __字符串 = __字符串.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
            if (__字符串.StartsWith("[{"))
            {
                return E数据结构.对象数组;
            }
            else if (__字符串.StartsWith("["))
            {
                return E数据结构.单值数组;
            }
            else if (__字符串.StartsWith("{"))
            {
                return E数据结构.对象;
            }
            return E数据结构.单值;
        }

        public static string AES解压(string __返回值)
        {
            using (var __目标流 = new MemoryStream())
            {
                using (var __源流 = new MemoryStream(Convert.FromBase64String(__返回值)))
                {
                    using (var __解压器 = new GZipStream(__源流, CompressionMode.Decompress))
                    {
                        __解压器.CopyTo(__目标流);
                    }
                }
                var __解压 = Encoding.UTF8.GetString(__目标流.GetBuffer());
                return __解压;
            }
        }

        public static string AES压缩(string __字符串)
        {
            using (var __目标流 = new MemoryStream())
            {
                using (var __源流 = new MemoryStream(Encoding.UTF8.GetBytes(__字符串)))
                {
                    using (var __压缩器 = new GZipStream(__目标流, CompressionMode.Compress))
                    {
                        __源流.CopyTo(__压缩器);
                    }
                }
                var __压缩 = Convert.ToBase64String(__目标流.GetBuffer());
                Debug.WriteLine("压缩率 {2}%, {0} > {1}", __字符串.Length, __压缩.Length, __压缩.Length * 100 / __字符串.Length);
                return __压缩;
            }
        }

    }
}
