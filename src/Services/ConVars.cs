/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Runtime.CompilerServices;
using SwiftlyS2.Shared.Convars;

namespace Identity;

public static class ConVars
{
    public static readonly IConVar<string> Url = Runtime.Core.ConVar.CreateOrFind(
        "identity_url",
        "URL endpoint for fetching player identity data.",
        ""
    );
    public static readonly IConVar<bool> IsStrict = Runtime.Core.ConVar.CreateOrFind(
        "identity_strict",
        "Kick players when their identity data cannot be retrieved.",
        true
    );
    public static readonly IConVar<bool> ForceNickname = Runtime.Core.ConVar.CreateOrFind(
        "identity_force_nickname",
        "Override player nicknames with their identity nickname.",
        true
    );
    public static readonly IConVar<bool> ForceRating = Runtime.Core.ConVar.CreateOrFind(
        "identity_force_rating",
        "Override player ratings with their identity rating.",
        true
    );

    public static void Initialize() =>
        RuntimeHelpers.RunClassConstructor(typeof(ConVars).TypeHandle);
}
