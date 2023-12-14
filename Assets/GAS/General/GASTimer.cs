﻿using System;

namespace GAS.General
{
    public class GASTimer
    {
        /// <summary>
        ///  The delta time between the server and the client.
        /// </summary>
        static long _deltaTimeWithServer;

        /// <summary>
        /// Notice: The time unit of this timestamp is milliseconds.
        /// Therefore, the time unit of the delta time is also milliseconds.
        /// </summary>
        /// <returns></returns>
        public static long Timestamp() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + _deltaTimeWithServer;

        public static long TimestampSeconds() => Timestamp() / 1000;
    }
}