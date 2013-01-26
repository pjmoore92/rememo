using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rememo
{
        public class DiaryManager
        {
            public const String DIARY_CONFIG_FILE = "diary.cfg";
            public const int DEFAULT_WEEKS = 4;
           

            /// <summary>
            /// Reads a config from the default config file, creating it if it doesn't exist.
            /// </summary>
            public static DiaryConfig ReadConfig()
            {
                if (!ConfigExists())
                {
                    DiaryConfig newConfig = new DiaryConfig();
                    newConfig.StartDate = DateTime.Now;
                    newConfig.Weeks = DEFAULT_WEEKS;
                    newConfig.Name = "User";

                    WriteConfig(newConfig);
                }

                List<String> lines = FileManager.ReadLines(DIARY_CONFIG_FILE);

                DiaryConfig config = new DiaryConfig();
                config.StartDate = DateTime.Parse(lines[0]);
                config.Weeks = int.Parse(lines[1]);
                config.Name = lines[2];

                return config;
            }

            /// <summary>
            /// Writes a reminder config to the default config file.
            /// </summary>
            public static void WriteConfig(DiaryConfig config)
            {
                String s = String.Format("{0}\n{1}\n{2}", config.StartDate.ToString(), config.Weeks, config.Name);

                FileManager.Write(DIARY_CONFIG_FILE, s);
            }

            /// <summary>
            /// Checks if the defualt config file exists;
            /// </summary>
            /// <returns></returns>
            public static bool ConfigExists()
            {
                return FileManager.Exists(DIARY_CONFIG_FILE);
            }
        }
    }
