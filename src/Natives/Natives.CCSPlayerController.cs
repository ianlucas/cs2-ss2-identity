/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Memory;

namespace Identity;

public static partial class Natives
{
    public delegate void CCSPlayerController_m_iszPlayerName1Delegate(nint a1, nint a2);

    public static readonly IUnmanagedFunction<CCSPlayerController_m_iszPlayerName1Delegate> CCSPlayerController_m_iszPlayerName1 =
        ResolveFunction<CCSPlayerController_m_iszPlayerName1Delegate>(
            "CCSPlayerController::m_iszPlayerName1"
        );

    public delegate nint CCSPlayerController_m_iszPlayerName2Delegate(
        nint a1,
        nint a2,
        byte a3,
        nint a4
    );

    public static readonly IUnmanagedFunction<CCSPlayerController_m_iszPlayerName2Delegate> CCSPlayerController_m_iszPlayerName2 =
        ResolveFunction<CCSPlayerController_m_iszPlayerName2Delegate>(
            "CCSPlayerController::m_iszPlayerName2"
        );

    public delegate nint CCSPlayerController_m_iszPlayerName3Delegate(nint a1, nint a2);

    public static readonly IUnmanagedFunction<CCSPlayerController_m_iszPlayerName3Delegate> CCSPlayerController_m_iszPlayerName3 =
        ResolveFunction<CCSPlayerController_m_iszPlayerName3Delegate>(
            "CCSPlayerController::m_iszPlayerName3"
        );

    public delegate nint CCSPlayerController_m_iszPlayerName4Delegate(nint a1);

    public static readonly IUnmanagedFunction<CCSPlayerController_m_iszPlayerName4Delegate> CCSPlayerController_m_iszPlayerName4 =
        ResolveFunction<CCSPlayerController_m_iszPlayerName4Delegate>(
            "CCSPlayerController::m_iszPlayerName4"
        );
}
