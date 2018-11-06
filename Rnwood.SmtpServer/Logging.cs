// <copyright file="Logging.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer
{
    using Microsoft.Extensions.Logging;

    internal static class Logging
    {
        public static ILoggerFactory Factory { get; } = new LoggerFactory();
    }
}
