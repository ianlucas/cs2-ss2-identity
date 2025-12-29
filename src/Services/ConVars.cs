/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Convars;

namespace Identity;

public static class ConVars
{
    [SwiftlyInject]
    private static ISwiftlyCore Core
    {
        get => _core!;
        set
        {
            _core = value;
            Initialize();
        }
    }
    private static ISwiftlyCore? _core;

    public static IConVar<string> Url { get; private set; } = null!;
    public static IConVar<bool> IsStrict { get; private set; } = null!;
    public static IConVar<bool> IsForceNickname { get; private set; } = null!;
    public static IConVar<bool> IsForceRating { get; private set; } = null!;

    private static void Initialize()
    {
        Url = Core.ConVar.Create(
            "identity_url",
            "URL endpoint for fetching player identity data.",
            ""
        );

        IsStrict = Core.ConVar.Create(
            "identity_strict",
            "Kick players when their identity data cannot be retrieved.",
            true
        );

        IsForceNickname = Core.ConVar.Create(
            "identity_force_nickname",
            "Override player nicknames with their identity nickname.",
            true
        );

        IsForceRating = Core.ConVar.Create(
            "identity_force_rating",
            "Override player ratings with their identity rating.",
            true
        );
    }
}
