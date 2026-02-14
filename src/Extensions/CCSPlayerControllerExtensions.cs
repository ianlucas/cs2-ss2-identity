/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Buffers;
using System.Text;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Identity;

public static class CCSPlayerControllerExtensions
{
    extension(CCSPlayerController self)
    {
        public unsafe void SetPlayerName(string name)
        {
            var pool = ArrayPool<byte>.Shared;
            var nameLength = Encoding.UTF8.GetByteCount(name);
            var nameBuffer = pool.Rent(nameLength + 1);
            try
            {
                _ = Encoding.UTF8.GetBytes(name, nameBuffer);
                nameBuffer[nameLength] = 0;
                fixed (byte* pName = nameBuffer)
                {
                    Natives.CCSPlayerController_SetPlayerName.Call(self.Address, (nint)pName);
                }
            }
            finally
            {
                pool.Return(nameBuffer);
            }
        }

        public void SetCompetitiveRanking(int rating)
        {
            self.CompetitiveRankType = 11;
            self.CompetitiveRankTypeUpdated();
            self.CompetitiveRanking = rating;
            self.CompetitiveRankingUpdated();
        }

        public void HideCompetitiveRanking()
        {
            self.CompetitiveRankType = 0;
            self.CompetitiveRankTypeUpdated();
        }
    }
}
