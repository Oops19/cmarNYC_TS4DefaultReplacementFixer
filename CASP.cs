using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TS4DefaultEyesFixer
{
    public class CASP       //Sims 4 CASP resource
    {
        uint version;	// 0x2A is current
        uint offset;	// to resource reference table from end of header (ie offset + 8)
        int presetCount; // Not used for TS4
        string partname;		// UnicodeBE - part name
        float sortPriority;	// CAS sorts on this value, from largest to smallest

        ushort swatchOrder;   // mSecondaryDisplayIndex - swatch order
        uint outfitID;    // Used to group CASPs
        uint materialHash;
        byte parameterFlags;  //parameter flags: 
        // 1 bit RestrictOppositeGender
        // 1 bit AllowForLiveRandom
        // 1 bit Show in CAS Demo
        // 1 bit ShowInSimInfoPanel
        // 1 bit ShowInUI
        // 1 bit AllowForCASRandom : 1;
        // 1 bit DefaultThumbnailPart
        // 1 bit Deprecated
        byte parameterFlags2; // additional parameter flags:
        // 5 bits unused
        // 1 bit DefaultForBodyTypeFemale
        // 1 bit DefaultForBodyTypeMale
        // 1 bit RestrictOppositeFrame
        ulong excludePartFlags; // parts removed
        ulong excludePartFlags2;         // v0x29
        ulong excludeModifierRegionFlags;
        int tagCount;
        PartTag[] categoryTags; // [tagCount] PartTags
        uint price;             //deprecated
        uint titleKey;
        uint partDescKey;
        uint unknown;
        byte textureSpace;
        uint bodyType;
        uint bodySubType;    // usually 8, not used before version 37
        AgeGender ageGender;
        uint species;          // added in version 0x20
        ushort packID;       // added in version 0x25
        byte packFlags;      // added in version 0x25
        // bit 7 - reserved, set to 0
        // bit 1 - hide pack icon
        byte[] Reserved2Set0;  // added in version 0x25, nine bytes set to 0
        byte Unused2;        //usually 1
        byte Unused3;        //if Unused2 > 0; usually 0
        byte usedColorCount;
        uint[] colorData;    // [usedColorCount] color code
        byte buffResKey;     // index to data file with custom icon and text info
        byte swatchIndex;    // index to swatch image
        ulong VoiceEffect;   // added in version 0x1C -  mVoiceEffectHash, a hash of a sound effect
        byte usedMaterialCount;       // added in version 0x1e - if not 0, should be 3
        uint materialSetUpperBodyHash;       // added in version 0x1e
        uint materialSetLowerBodyHash;       // added in version 0x1e
        uint materialSetShoesHash;       // added in version 0x1e 
        uint occultBitField;            // added in version 0x1f - disabled for occult types
        // 30 bits reserved
        //  1 bit alien
        //  1 bit human
        UInt64 oppositeGenderPart;      // Version 0x28 - If the current part is not compatible with the Sim due to frame/gender
        // restrictions, use this part instead. Maxis convention is to use this
        // to specify the opposite gender version of the part. Set to 0 for none.
        UInt64 fallbackPart;            // Version 0x28 - If the current part is not compatible with the Sim due to frame/gender
        // restrictions, and there is no mOppositeGenderPart specified, use this part.
        // Maxis convention is to use this to specify a replacement part which is not
        // necessarily the opposite gendered version of the part. Set to 0 for none.
        byte nakedKey;
        byte parentKey;
        int sortLayer;
        byte lodCount;
        MeshDesc[] lods;      // [count] mesh lod and part indexes 
        byte numSlotKeys;
        byte[] slotKeys;      // [numSlotKeys] bytes
        byte textureIndex;    // index to texture TGI (diffuse)
        byte shadowIndex;     // index to 'shadow' texture/overlay
        byte compositionMethod;
        byte regionMapIndex;  // index to RegionMap file
        byte numOverrides;
        Override[] overrides; // [numOverrides] Override
        byte normalMapIndex;
        byte specularIndex;   // DDSRLES 
        uint UVoverride;      //added in version 0x1b, so far same values as bodyType
        byte emissionIndex;   // added in version 0x1d, for alien glow 
        byte reserved;        // added in version 0x2A
        byte IGTcount;        // Resource reference table in I64GT format (not TGI64)
        // --repeat(count)
        TGI[] IGTtable;

        public BodyType BodyType
        {
            get { return (BodyType)this.bodyType; }
            set { this.bodyType = (uint)value; }
        }

        public byte TextureIndex
        {
            get { return this.textureIndex; }
            set { this.textureIndex = (byte)value; }
        }

        public TGI[] LinkList
        {
            get { return IGTtable; }
            set { IGTtable = value; }
        }

        public CASP(BinaryReader br)
        {
            br.BaseStream.Position = 0;
            if (br.BaseStream.Length < 32) throw new CASPEmptyException("Attempt to read empty CASP");
            version = br.ReadUInt32();
            offset = br.ReadUInt32();
            presetCount = br.ReadInt32();
            partname = new BinaryReader(br.BaseStream, Encoding.BigEndianUnicode).ReadString();
            sortPriority = br.ReadSingle();
            swatchOrder = br.ReadUInt16();
            outfitID = br.ReadUInt32();
            materialHash = br.ReadUInt32();
            parameterFlags = br.ReadByte();
            if (this.version >= 39) parameterFlags2 = br.ReadByte();
            excludePartFlags = br.ReadUInt64();
            if (version >= 41)
            {
                excludePartFlags2 = br.ReadUInt64();
            }
            if (version > 36)
            {
                excludeModifierRegionFlags = br.ReadUInt64();
            }
            else
            {
                excludeModifierRegionFlags = br.ReadUInt32();
            }
            tagCount = br.ReadInt32();
            categoryTags = new PartTag[tagCount];
            for (int i = 0; i < tagCount; i++)
            {
                categoryTags[i] = new PartTag(br, version >= 37 ? 4 : 2);
            }
            price = br.ReadUInt32();
            titleKey = br.ReadUInt32();
            partDescKey = br.ReadUInt32();
            if (version >= 43) unknown = br.ReadUInt32();
            textureSpace = br.ReadByte();
            bodyType = br.ReadUInt32();
            bodySubType = br.ReadUInt32();
            ageGender = new AgeGender(br);
            if (this.version >= 32) species = br.ReadUInt32();
            if (this.version >= 34)
            {
                packID = br.ReadUInt16();
                packFlags = br.ReadByte();
                Reserved2Set0 = br.ReadBytes(9);
            }
            else
            {
                Unused2 = br.ReadByte();
                if (Unused2 > 0) Unused3 = br.ReadByte();
            }
            usedColorCount = br.ReadByte();
            colorData = new uint[usedColorCount];
            for (int i = 0; i < usedColorCount; i++)
            {
                colorData[i] = br.ReadUInt32();
            }
            buffResKey = br.ReadByte();
            swatchIndex = br.ReadByte();
            if (version >= 28)
            {
                VoiceEffect = br.ReadUInt64();
            }
            if (version >= 30)
            {
                usedMaterialCount = br.ReadByte();
                if (usedMaterialCount > 0)
                {
                    materialSetUpperBodyHash = br.ReadUInt32();
                    materialSetLowerBodyHash = br.ReadUInt32();
                    materialSetShoesHash = br.ReadUInt32();
                }
            }
            if (version >= 31)
            {
                occultBitField = br.ReadUInt32();
            }
            if (version >= 38)
            {
                oppositeGenderPart = br.ReadUInt64();
            }
            if (version >= 39)
            {
                fallbackPart = br.ReadUInt64();
            }
            nakedKey = br.ReadByte();
            parentKey = br.ReadByte();
            sortLayer = br.ReadInt32();
            lodCount = br.ReadByte();
            lods = new MeshDesc[lodCount];
            for (int i = 0; i < lodCount; i++)
            {
                lods[i] = new MeshDesc(br);
            }
            numSlotKeys = br.ReadByte();
            slotKeys = br.ReadBytes(numSlotKeys);
            textureIndex = br.ReadByte();
            shadowIndex = br.ReadByte();
            compositionMethod = br.ReadByte();
            regionMapIndex = br.ReadByte();
            numOverrides = br.ReadByte();
            overrides = new Override[numOverrides];
            for (int i = 0; i < numOverrides; i++)
            {
                overrides[i] = new Override(br);
            }
            normalMapIndex = br.ReadByte();
            specularIndex = br.ReadByte();
            if (version >= 27)
            {
                UVoverride = br.ReadUInt32();
            }
            if (version >= 29)
            {
                emissionIndex = br.ReadByte();
            }
            if (version >= 42)
            {
                reserved = br.ReadByte();
            }
            IGTcount = br.ReadByte();
            IGTtable = new TGI[IGTcount];
            for (int i = 0; i < IGTcount; i++)
            {
                IGTtable[i] = new TGI(br, TGI.TGIsequence.IGT);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(version);
            long offsetPos = bw.BaseStream.Position;
            bw.Write(0);
            bw.Write(presetCount);
            new BinaryWriter(bw.BaseStream, Encoding.BigEndianUnicode).Write(partname);
            bw.Write(sortPriority);
            bw.Write(swatchOrder);
            bw.Write(outfitID);
            bw.Write(materialHash);
            bw.Write(parameterFlags);
            if (this.version >= 39) bw.Write(parameterFlags2);
            bw.Write(excludePartFlags);
            if (version >= 41)
            {
                bw.Write(excludePartFlags2);
            }
            if (version > 36)
            {
                bw.Write(excludeModifierRegionFlags);
            }
            else
            {
                bw.Write((uint)excludeModifierRegionFlags);
            }
            bw.Write(tagCount);
            for (int i = 0; i < tagCount; i++)
            {
                categoryTags[i].Write(bw, version >= 37 ? 4 : 2);
            }
            bw.Write(price);
            bw.Write(titleKey);
            bw.Write(partDescKey);
            if (version >= 43) bw.Write(unknown);
            bw.Write(textureSpace);
            bw.Write(bodyType);
            bw.Write(bodySubType);
            ageGender.Write(bw);
            if (this.version >= 32) bw.Write(species);
            if (this.version >= 34)
            {
                bw.Write(packID);
                bw.Write(packFlags);
                bw.Write(Reserved2Set0);
            }
            else
            {
                bw.Write(Unused2);
                if (Unused2 > 0) bw.Write(Unused3);
            }
            bw.Write(usedColorCount);
            for (int i = 0; i < usedColorCount; i++)
            {
                bw.Write(colorData[i]);
            }
            bw.Write(buffResKey);
            bw.Write(swatchIndex);
            if (version >= 28)
            {
                bw.Write(VoiceEffect);
            }
            if (version >= 30)
            {
                bw.Write(usedMaterialCount);
                if (usedMaterialCount > 0)
                {
                    bw.Write(materialSetUpperBodyHash);
                    bw.Write(materialSetLowerBodyHash);
                    bw.Write(materialSetShoesHash);
                }
            }
            if (version >= 31)
            {
                bw.Write(occultBitField);
            }
            if (version >= 38)
            {
                bw.Write(oppositeGenderPart);
            }
            if (version >= 39)
            {
                bw.Write(fallbackPart);
            }
            bw.Write(nakedKey);
            bw.Write(parentKey);
            bw.Write(sortLayer);
            bw.Write(lodCount);
            for (int i = 0; i < lodCount; i++)
            {
                lods[i].Write(bw);
            }
            bw.Write(numSlotKeys);
            bw.Write(slotKeys);
            bw.Write(textureIndex);
            bw.Write(shadowIndex);
            bw.Write(compositionMethod);
            bw.Write(regionMapIndex);
            bw.Write(numOverrides);
            for (int i = 0; i < numOverrides; i++)
            {
                overrides[i].Write(bw);
            }
            bw.Write(normalMapIndex);
            bw.Write(specularIndex);
            if (version >= 27)
            {
                bw.Write(UVoverride);
            }
            if (version >= 29)
            {
                bw.Write(emissionIndex);
            }
            if (version >= 42)
            {
                bw.Write(reserved);
            }
            long tablePos = bw.BaseStream.Position;
            bw.BaseStream.Position = offsetPos;
            bw.Write((uint)(tablePos - 8));
            bw.BaseStream.Position = tablePos;
            bw.Write((byte)IGTtable.Length);
            for (int i = 0; i < IGTtable.Length; i++)
            {
                IGTtable[i].Write(bw, TGI.TGIsequence.IGT);
            }
        }

        public void RemoveSpecular()
        {
            this.specularIndex = RemoveKey(this.specularIndex);
            this.RebuildLinkList();
        }

        internal byte RemoveKey(byte index)
        {
            TGI empty_tgi = new TGI(0, 0, 0);
            if (this.IGTtable[index].Equals(empty_tgi) || index < 0 || index > this.IGTtable.Length - 1) return index;  //if not needed or invalid index, don't do anything.
            byte empty_index = 0;
            bool found = false;
            for (int i = 0; i < this.IGTtable.Length; i++)
            {
                if (this.IGTtable[i].Equals(empty_tgi))
                {
                    empty_index = (byte)i;
                    found = true;
                    break;
                }
            }

            if (found)
            {
                return empty_index;
            }
            else
            {
                TGI[] newTGIlist = new TGI[this.IGTtable.Length + 1];
                Array.Copy(this.IGTtable, newTGIlist, this.IGTtable.Length);
                newTGIlist[this.IGTtable.Length] = empty_tgi;
                this.IGTtable = newTGIlist;
                return (byte)(this.IGTtable.Length - 1);
            }
        }

        public void RebuildLinkList()
        {
            List<TGI> newLinks = new List<TGI>();
            newLinks.Add(new TGI(0, 0, 0));

            this.buffResKey = AddLink(this.buffResKey, newLinks);
            this.swatchIndex = AddLink(this.swatchIndex, newLinks);
            this.nakedKey = AddLink(this.nakedKey, newLinks);
            this.parentKey = AddLink(this.parentKey, newLinks);
            for (int i = 0; i < this.lodCount; i++)
            {
                for (int j = 0; j < this.lods[i].indexes.Length; j++)
                {
                    this.lods[i].indexes[j] = AddLink(this.lods[i].indexes[j], newLinks);
                }
            }
            for (int i = 0; i < this.numSlotKeys; i++)
            {
                this.slotKeys[i] = AddLink(this.slotKeys[i], newLinks);
            }
            this.textureIndex = AddLink(this.textureIndex, newLinks);
            this.shadowIndex = AddLink(this.shadowIndex, newLinks);
            this.regionMapIndex = AddLink(this.regionMapIndex, newLinks);
            this.normalMapIndex = AddLink(this.normalMapIndex, newLinks);
            this.specularIndex = AddLink(this.specularIndex, newLinks);
            this.emissionIndex = AddLink(this.emissionIndex, newLinks);
            this.IGTtable = newLinks.ToArray();
        }

        internal byte AddLink(byte indexIn, List<TGI> linkList)
        {
            if (this.IGTtable[indexIn].Equals(new TGI(0, 0, 0)) | indexIn < 0 | indexIn > IGTtable.Length) // correct invalid links
            {
                return (byte)0;
            }
            else
            {
                byte tmp = (byte)linkList.Count;
                linkList.Add(this.IGTtable[indexIn]);
                return tmp;
            }
        }

        internal class AgeGender
        {
            byte age;
            byte gender;
            UInt16 unused;

            public Age Age
            {
                get
                {
                    return (Age)age;
                }
                set
                {
                    this.age = (byte)value;
                }
            }
            public int AgeNumeric
            {
                get
                {
                    return age;
                }
            }
            public Gender Gender
            {
                get
                {
                    return (Gender)(byte)(this.gender >> 4);
                }
                set
                {
                    this.gender = (byte)((byte)value << 4);
                }

            }
            public int GenderNumeric
            {
                get
                {
                    return (byte)(this.gender >> 4);
                }
            }

            public AgeGender(Age Age, Gender Gender)
            {
                this.age = (byte)Age;
                this.gender = (byte)((byte)Gender << 4);
            }

            internal AgeGender(BinaryReader br)
            {
                this.age = br.ReadByte();
                this.gender = br.ReadByte();
                this.unused = br.ReadUInt16();
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write(this.age);
                bw.Write(this.gender);
                bw.Write(this.unused);
            }
        }

        internal class PartTag
        {
            ushort flagCategory;
            uint flagValue;

            internal ushort FlagCategory
            {
                get { return this.flagCategory; }
                set { this.flagCategory = value; }
            }
            internal uint FlagValue
            {
                get { return this.flagValue; }
                set { this.flagValue = value; }
            }

            internal PartTag(BinaryReader br, int valueLength)
            {
                flagCategory = br.ReadUInt16();
                if (valueLength == 4)
                {
                    flagValue = br.ReadUInt32();
                }
                else
                {
                    flagValue = br.ReadUInt16();
                }
            }

            internal PartTag(ushort category, uint flagVal)
            {
                this.flagCategory = category;
                this.flagValue = flagVal;
            }

            internal void Write(BinaryWriter bw, int valueLength)
            {
                bw.Write(flagCategory);
                if (valueLength == 4)
                {
                    bw.Write(flagValue);
                }
                else
                {
                    bw.Write((ushort)flagValue);
                }
            }
        }

        internal class Override
        {
            byte region;
            float layer;

            internal Override(BinaryReader br)
            {
                region = br.ReadByte();
                layer = br.ReadSingle();
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write(region);
                bw.Write(layer);
            }
        }

        internal class MeshDesc
        {
            internal byte lod;
            internal uint Unused1;
            LODasset[] assets;
            internal byte[] indexes;

            internal int Length
            {
                get
                {
                    return 7 + (12 * assets.Length) + indexes.Length;
                }
            }

            internal void removeMeshLink(TGI meshTGI, TGI[] caspTGIlist)
            {
                List<byte> tmp = new List<byte>();
                foreach (byte i in indexes)
                {
                    if (!meshTGI.Equals(caspTGIlist[i])) tmp.Add(i);
                }
                this.indexes = tmp.ToArray();
            }

            internal void addMeshLink(TGI meshTGI, TGI[] caspTGIlist)
            {
                List<byte> tmp = new List<byte>(this.indexes);
                for (byte i = 0; i < caspTGIlist.Length; i++)
                {
                    if (meshTGI.Equals(caspTGIlist[i]))
                    {
                        tmp.Add(i);
                        break;
                    }
                }
                this.indexes = tmp.ToArray();
            }

            internal MeshDesc(BinaryReader br)
            {
                lod = br.ReadByte();
                Unused1 = br.ReadUInt32();
                byte numAssets = br.ReadByte();
                assets = new LODasset[numAssets];
                for (int i = 0; i < numAssets; i++)
                {
                    assets[i] = new LODasset(br);
                }
                byte indexCount = br.ReadByte();
                indexes = new byte[indexCount];
                for (int i = 0; i < indexCount; i++)
                {
                    indexes[i] = br.ReadByte();
                }
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write(lod);
                bw.Write(Unused1);
                bw.Write((byte)assets.Length);
                for (int i = 0; i < assets.Length; i++)
                {
                    assets[i].Write(bw);
                }
                bw.Write((byte)indexes.Length);
                for (int i = 0; i < indexes.Length; i++)
                {
                    bw.Write(indexes[i]);
                }
            }

            internal class LODasset
            {
                internal int sorting;
                internal int specLevel;
                internal int castShadow;

                internal LODasset(BinaryReader br)
                {
                    this.sorting = br.ReadInt32();
                    this.specLevel = br.ReadInt32();
                    this.castShadow = br.ReadInt32();
                }

                internal void Write(BinaryWriter bw)
                {
                    bw.Write(this.sorting);
                    bw.Write(this.specLevel);
                    bw.Write(this.castShadow);
                }
            }
        }

        [global::System.Serializable]
        public class CASPEmptyException : ApplicationException
        {
            public CASPEmptyException() { }
            public CASPEmptyException(string message) : base(message) { }
            public CASPEmptyException(string message, Exception inner) : base(message, inner) { }
            protected CASPEmptyException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
    }

    [Flags]
    public enum Age
    {
        None = 0,
        Baby = 1,
        Toddler = 2,
        BabyAndToddler = 3,
        Child = 4,
        ToddlerToChild = 6,
        BabyToChild = 7,
        Teen = 8,
        YoungAdult = 16,
        TeenAndYA = 24,
        Adult = 32,
        YAandAdult = 48,
        TeenToAdult = 56,
        ChildToAdult = 60,
        ToddlerToAdult = 62,
        BabyToAdult = 63,
        Elder = 64,
        AdultAndElder = 96,
        YAtoElder = 112,
        TeenToElder = 120,
        ChildToElder = 124,
        ToddlerToElder = 126,
        AllAges = 127
    }

    [Flags]
    public enum Gender
    {
        None = 0,
        Male = 1,
        Female = 2,
        Unisex = 3
    }

    public enum BodyType        //used in TS4
    {
        All = 0,
        Hat = 1,
        Hair = 2,
        Head = 3,
        Face = 4,
        Body = 5,
        Top = 6,
        Bottom = 7,
        Shoes = 8,
        Accessories = 9,
        Earrings = 0x0A,
        Glasses = 0x0B,
        Necklace = 0x0C,
        Gloves = 0x0D,
        BraceletLeft = 0x0E,
        BraceletRight = 0x0F,
        LipRingLeft = 0x10,
        LipRingRight = 0x11,
        NoseRingLeft = 0x12,
        NoseRingRight = 0x13,
        BrowRingLeft = 0x14,
        BrowRingRight = 0x15,
        RingIndexLeft = 0x16,
        RingIndexRight = 0x17,
        RingThirdLeft = 0x18,
        RingThirdRight = 0x19,
        RingMidLeft = 0x1A,
        RingMidRight = 0x1B,
        FacialHair = 0x1C,
        Lipstick = 0x1D,
        Eyeshadow = 0x1E,
        Eyeliner = 0x1F,
        Blush = 0x20,
        Facepaint = 0x21,
        Eyebrows = 0x22,
        Eyecolor = 0x23,
        Socks = 0x24,
        Mascara = 0x25,
        ForeheadCrease = 0x26,
        Freckles = 0x27,
        DimpleLeft = 0x28,
        DimpleRight = 0x29,
        Tights = 0x2A,
        MoleLeftLip = 0x2B,
        MoleRightLip = 0x2C,
        TattooArmLowerLeft = 0x2D,
        TattooArmUpperLeft = 0x2E,
        TattooArmLowerRight = 0x2F,
        TattooArmUpperRight = 0x30,
        TattooLegLeft = 0x31,
        TattooLegRight = 0x32,
        TattooTorsoBackLower = 0x33,
        TattooTorsoBackUpper = 0x34,
        TattooTorsoFrontLower = 0x35,
        TattooTorsoFrontUpper = 0x36,
        MoleLeftCheek = 0x37,
        MoleRightCheek = 0x38,
        MouthCrease = 0x39,
        SkinOverlay = 0x3A,
        Fur = 0x3B,
        AnimalEars = 0x3C,
        Tail = 0x3D,
        NoseColor = 0x3E,
        SecondaryEyeColor = 0x3F,
        OccultBrow = 0x40,
        OccultEyeSocket = 0x41,
        OccultEyeLid = 0x42,
        OccultMouth = 0x43,
        OccultLeftCheek = 0x44,
        OccultRightCheek = 0x45,
        OccultNeckScar = 0x46,
        SkinDetailScar = 0x47,
        SkinDetailAcne = 0x48
    }
}
