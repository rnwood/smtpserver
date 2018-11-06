﻿// <copyright file="IExtensionProcessor.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer.Extensions
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="IExtensionProcessor" />
    /// </summary>
    public interface IExtensionProcessor
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        Task<string[]> GetEHLOKeywords();
    }
}
