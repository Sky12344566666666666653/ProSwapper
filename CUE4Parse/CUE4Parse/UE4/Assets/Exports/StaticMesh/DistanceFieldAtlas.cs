using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Assets.Exports.StaticMesh
{
    public static class DistanceField
    {
        public const int NumMips = 3;
    }

    public struct FSparseDistanceFieldMip
    {
        public FIntVector IndirectionDimensions;
        public int NumDistanceFieldBricks;
        public FVector VolumeToVirtualUVScale;
        public FVector VolumeToVirtualUVAdd;
        public FVector2D DistanceFieldToVolumeScaleBias;
        public uint BulkOffset;
        public uint BulkSize;
    }

    public class FDistanceFieldVolumeData
    {
        public ushort[] DistanceFieldVolume; // LegacyIndices
        public FIntVector Size;
        public FBox LocalBoundingBox;
        public bool bMeshWasClosed;
        public bool bBuiltAsIfTwoSided;
        public bool bMeshWasPlane;

        public byte[] CompressedDistanceFieldVolume;
        public FVector2D DistanceMinMax;

        public FDistanceFieldVolumeData(FArchive Ar)
        {
            if (Ar.Game >= EGame.GAME_UE4_16)
            {
                CompressedDistanceFieldVolume = Ar.ReadArray<byte>();
                Size = Ar.Read<FIntVector>();
                LocalBoundingBox = Ar.Read<FBox>();
                DistanceMinMax = Ar.Read<FVector2D>();
                bMeshWasClosed = Ar.ReadBoolean();
                bBuiltAsIfTwoSided = Ar.ReadBoolean();
                bMeshWasPlane = Ar.ReadBoolean();
                DistanceFieldVolume = new ushort[0];
            }
            else
            {
                DistanceFieldVolume = Ar.ReadArray<ushort>();
                Size = Ar.Read<FIntVector>();
                LocalBoundingBox = Ar.Read<FBox>();
                bMeshWasClosed = Ar.ReadBoolean();
                bBuiltAsIfTwoSided = Ar.Ver >= UE4Version.VER_UE4_RENAME_CROUCHMOVESCHARACTERDOWN && Ar.ReadBoolean();
                bMeshWasPlane = Ar.Ver >= UE4Version.VER_UE4_DEPRECATE_UMG_STYLE_ASSETS && Ar.ReadBoolean();
                CompressedDistanceFieldVolume = new byte[0];
                DistanceMinMax = new FVector2D(0f, 0f);
            }
        }
    }

    public class FDistanceFieldVolumeData5
    {
        /** Local space bounding box of the distance field volume. */
        public FBox LocalSpaceMeshBounds;

        /** Whether most of the triangles in the mesh used a two-sided material. */
        public bool bMostlyTwoSided;

        public FSparseDistanceFieldMip[] Mips;

        // Lowest resolution mip is always loaded so we always have something
        public byte[] AlwaysLoadedMip;

        // Remaining mips are streamed
        public FByteBulkData StreamableMips;

        public FDistanceFieldVolumeData5(FAssetArchive Ar)
        {
            LocalSpaceMeshBounds = Ar.Read<FBox>();
            bMostlyTwoSided = Ar.ReadBoolean();
            Mips = Ar.ReadArray<FSparseDistanceFieldMip>(DistanceField.NumMips);
            AlwaysLoadedMip = Ar.ReadArray<byte>();
            StreamableMips = new FByteBulkData(Ar);
        }
    }
}