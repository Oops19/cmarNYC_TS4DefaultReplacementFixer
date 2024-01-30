/* TS4 Default Eyes Fixer, a tool for creating custom content for The Sims 4,
   Copyright (C) 2020  C. Marinetti

   This program is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
   The author may be contacted at modthesims.info, username cmarNYC. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using s4pi.Interfaces;
using s4pi.Package;

namespace TS4DefaultEyesFixer
{
    public partial class Form1 : Form
    {
        List<CASP> defCaspList = null;
        List<IResourceIndexEntry> caspires = null;

        public Form1()
        {
            InitializeComponent();

            Package DefaultCasps = null;
            try
            {
                DefaultCasps = (Package)Package.OpenPackage(1, "DefaultCASPs.package");
            }
            catch
            {
                DialogResult res = MessageBox.Show("DefaultCASPS.package not found! Can't add CASPs for texture-only replacements or process CASP replacements." + Environment.NewLine + "Continue anyway?",
                    "Defaults package not found", MessageBoxButtons.OKCancel);
            }
            if (DefaultCasps != null)
            {
                defCaspList = new List<CASP>();
                Predicate<IResourceIndexEntry> casps = r => r.ResourceType == 0x034AEECB;       //casp
                caspires = DefaultCasps.FindAll(casps);
                for (int i = 0; i < caspires.Count; i++)
                {
                    Stream d = DefaultCasps.GetResource(caspires[i]);
                    CASP casp = new CASP(new BinaryReader(d));
                    defCaspList.Add(casp);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!TextureOnly_radioButton.Checked && !CASPs_radioButton.Checked)
            {
                MessageBox.Show("Please select texture-only or with CASPs first!");
                return;
            }
            string packFile = GetFilename("Select default replacement package", "Package files (*.package)|*.package|All files (*.*)|*.*");
            if (!File.Exists(packFile))
            {
                MessageBox.Show("You have not selected a valid package file!");
                return;
            }
            Package pack = (Package)Package.OpenPackage(1, packFile, false);
            if (pack == null)
            {
                MessageBox.Show("Cannot read package file!");
                return;
            }
            string filename = Path.GetFileNameWithoutExtension(packFile) + "_Fixed.package";
            Package newPack = (Package)Package.NewPackage(1);

            if (TextureOnly_radioButton.Checked)
            {
                ConvertToTextures(pack, newPack, filename);
            }
            else
            {
                ConvertToCASPs(pack, newPack, filename);
            }
        }

        private void ConvertToTextures(Package pack, Package newPack, string filename)
        {
            int defaultCount = 0;
            Predicate<IResourceIndexEntry> casps = r => r.ResourceType == 0x034AEECB;       //casp
            List<IResourceIndexEntry> iries = pack.FindAll(casps);

            for (int i = 0; i < iries.Count; i++)
            {
                Stream c = pack.GetResource(iries[i]);
                CASP casp = new CASP(new BinaryReader(c));
                TGI imgTGI = casp.LinkList[casp.TextureIndex];
                Predicate<IResourceIndexEntry> imgIRE = r => r.ResourceType == imgTGI.Type & r.ResourceGroup == imgTGI.Group & r.Instance == imgTGI.Instance;
                IResourceIndexEntry irie = pack.Find(imgIRE);
                Stream si = null;
                if (irie != null) si = pack.GetResource(irie);

                if (caspires != null && si != null)
                {
                    for (int j = 0; j < caspires.Count; j++)
                    {
                        if (caspires[j].Instance == iries[i].Instance)
                        {
                            TGI defTGI = defCaspList[j].LinkList[defCaspList[j].TextureIndex];
                            TGIBlock newTGI = new TGIBlock(1, null, 0x2BC04EDF, defTGI.Group, defTGI.Instance);
                            if (Copy_checkBox.Checked)
                            {
                                IResourceIndexEntry res = newPack.AddResource(newTGI, si, true);
                                res.Compressed = (ushort)0x5A42;
                                defaultCount++;
                            }
                            else
                            {
                                DeleteResource(pack, irie);
                                IResourceIndexEntry res = pack.AddResource(newTGI, si, true);
                                res.Compressed = (ushort)0x5A42;
                                DeleteResource(pack, iries[i]);
                                defaultCount++;
                                break;
                            }
                        }
                    }
                }
                else if (si != null)
                {
                    TGIBlock newTGI = new TGIBlock(1, null, 0x2BC04EDF, imgTGI.Group, imgTGI.Instance);
                    if (Copy_checkBox.Checked)
                    {
                        IResourceIndexEntry res = newPack.AddResource(newTGI, si, true);
                        res.Compressed = (ushort)0x5A42;
                        defaultCount++;
                    }
                    else
                    {
                        DeleteResource(pack, irie);
                        IResourceIndexEntry res = pack.AddResource(newTGI, si, true);
                        res.Compressed = (ushort)0x5A42;
                        DeleteResource(pack, iries[i]);
                        defaultCount++;
                        break;
                    }
                }
            }

            Predicate<IResourceIndexEntry> texIRE = r => r.ResourceType == 0x3453CF95;
            List<IResourceIndexEntry> irieTex = pack.FindAll(texIRE);

            if (irieTex != null)
            {
                for (int i = 0; i < irieTex.Count; i++)
                {
                    Stream s = pack.GetResource(irieTex[i]);
                    TGIBlock newTGI = new TGIBlock(1, null, 0x2BC04EDF, irieTex[i].ResourceGroup, irieTex[i].Instance);
                    if (Copy_checkBox.Checked)
                    {
                        IResourceIndexEntry res2 = newPack.AddResource(newTGI, s, true);
                        res2.Compressed = (ushort)0x5A42;
                    }
                    else
                    {
                        DeleteResource(pack, irieTex[i]);
                        IResourceIndexEntry res2 = pack.AddResource(newTGI, s, true);
                        res2.Compressed = (ushort)0x5A42;
                    }
                    defaultCount++;
                }
            }

            MessageBox.Show(defaultCount.ToString() + " default replacements updated");
            if (Copy_checkBox.Checked)
            {
                if (!WritePackage("Save as new package", newPack, filename))
                {
                    MessageBox.Show("Could not save new package!");
                    return;
                }
            }
            else
            {
                if (!WritePackage("Save as new package", pack, filename))
                {
                    MessageBox.Show("Could not save new package!");
                    return;
                }
            }
        }
        
        private void ConvertToCASPs(Package pack, Package newPack, string filename)
        {
            int defaultCount = 0;
            Predicate<IResourceIndexEntry> casps = r => r.ResourceType == 0x034AEECB;       //casp
            List<IResourceIndexEntry> iries = pack.FindAll(casps);

            for (int i = 0; i < iries.Count; i++)
            {
                Stream c = pack.GetResource(iries[i]);
                CASP casp = new CASP(new BinaryReader(c));
                if (casp.BodyType == BodyType.Eyecolor || casp.BodyType == BodyType.SecondaryEyeColor ||
                    casp.BodyType == BodyType.Eyebrows || casp.BodyType == BodyType.Blush || casp.BodyType == BodyType.Eyeliner ||
                    casp.BodyType == BodyType.Eyeshadow || casp.BodyType == BodyType.Lipstick || casp.BodyType == BodyType.Mascara || 
                    casp.BodyType == BodyType.OccultBrow || casp.BodyType == BodyType.OccultEyeLid || casp.BodyType == BodyType.OccultEyeSocket || 
                    casp.BodyType == BodyType.OccultLeftCheek || casp.BodyType == BodyType.OccultMouth || casp.BodyType == BodyType.OccultNeckScar || 
                    casp.BodyType == BodyType.OccultRightCheek)
                {
                    TGI imgTGI = casp.LinkList[casp.TextureIndex];
                    Predicate<IResourceIndexEntry> imgIRE = r => r.ResourceType == imgTGI.Type & r.ResourceGroup == imgTGI.Group & r.Instance == imgTGI.Instance;
                    IResourceIndexEntry irie = pack.Find(imgIRE);
                    if (irie == null) continue;
                    Stream si = pack.GetResource(irie);
                    TGIBlock newTGI = new TGIBlock(1, null, imgTGI.Type, imgTGI.Group | 0x80000000, imgTGI.Instance);
                    if (Copy_checkBox.Checked)
                    {
                        IResourceIndexEntry res = newPack.AddResource(newTGI, si, true);
                        res.Compressed = (ushort)0x5A42;
                    }
                    else
                    {
                        DeleteResource(pack, irie);
                        IResourceIndexEntry res = pack.AddResource(newTGI, si, true);
                        res.Compressed = (ushort)0x5A42;
                    }

                    casp.LinkList[casp.TextureIndex] = new TGI(newTGI.ResourceType, newTGI.ResourceGroup, newTGI.Instance);
                    if (Shine_checkBox.Checked) casp.RemoveSpecular();
                    Stream s = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(s);
                    casp.Write(bw);
                    if (Copy_checkBox.Checked)
                    {
                        IResourceIndexEntry res2 = newPack.AddResource(iries[i], s, true);
                        res2.Compressed = (ushort)0x5A42;
                    }
                    else
                    {
                        DeleteResource(pack, iries[i]);
                        IResourceIndexEntry res2 = pack.AddResource(iries[i], s, true);
                        res2.Compressed = (ushort)0x5A42;
                    }
                    defaultCount++;
                }
            }

            if (defCaspList != null)
            {
                Predicate<IResourceIndexEntry> tex = r => r.ResourceType == 0x3453CF95 & r.ResourceGroup == 0u;       //RLE texture
                List<IResourceIndexEntry> texires = pack.FindAll(tex);
                for (int i = 0; i < texires.Count; i++)
                {
                    bool found = false;
                    for (int j = 0; j < defCaspList.Count; j++)
                    {
                        TGI tgiLink = defCaspList[j].LinkList[defCaspList[j].TextureIndex];
                        if (tgiLink.Instance == texires[i].Instance && tgiLink.Group == 0)
                        {
                            Stream si = pack.GetResource(texires[i]);
                            TGIBlock newTGI = new TGIBlock(1, null, texires[i].ResourceType, texires[i].ResourceGroup | 0x80000000, texires[i].Instance);
                            if (Copy_checkBox.Checked)
                            {
                                IResourceIndexEntry rest = newPack.AddResource(newTGI, si, true);
                                rest.Compressed = (ushort)0x5A42;
                            }
                            else
                            {
                                DeleteResource(pack, texires[i]);
                                IResourceIndexEntry rest = pack.AddResource(newTGI, si, true);
                                rest.Compressed = (ushort)0x5A42;
                            }

                            defCaspList[j].LinkList[defCaspList[j].TextureIndex] = new TGI(newTGI.ResourceType, newTGI.ResourceGroup, newTGI.Instance);
                            if (Shine_checkBox.Checked) defCaspList[j].RemoveSpecular();
                            Stream s = new MemoryStream();
                            BinaryWriter bw = new BinaryWriter(s);
                            defCaspList[j].Write(bw);
                            if (Copy_checkBox.Checked)
                            {
                                IResourceIndexEntry rest2 = newPack.AddResource(caspires[j], s, true);
                                rest2.Compressed = (ushort)0x5A42;
                            }
                            else
                            {
                                IResourceIndexEntry rest2 = pack.AddResource(caspires[j], s, true);
                                rest2.Compressed = (ushort)0x5A42;
                            }

                            found = true;
                        }
                    }
                    if (found) defaultCount++;
                }
            }

            MessageBox.Show(defaultCount.ToString() + " default replacements updated");
            if (Copy_checkBox.Checked)
            {
                if (!WritePackage("Save as new package", newPack, filename))
                {
                    MessageBox.Show("Could not save new package!");
                    return;
                }
            }
            else
            {
                if (!WritePackage("Save as new package", pack, filename))
                {
                    MessageBox.Show("Could not save new package!");
                    return;
                }
            }
        }

        internal string GetFilename(string title, string filter)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = filter;
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.Title = title;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            else
            {
                return "";
            }
        }

        internal bool WritePackage(string title, Package pack, string defaultName)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Package files (*.package)|*.package|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.Title = title;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "package";
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.FileName = defaultName;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    int i = 1;
                    string n = Path.GetDirectoryName(saveFileDialog1.FileName) + '\\' + Path.GetFileNameWithoutExtension(saveFileDialog1.FileName);
                    string num = "";
                    bool rename = false;
                    while (File.Exists(n + num + ".package"))
                    {
                        num = i.ToString();
                        i++;
                        rename = true;
                    }
                    pack.SaveAs(n + num + ".package");
                    if (rename) MessageBox.Show("Package saved as " + n + num + ".package");
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file " + saveFileDialog1.FileName + ". Original error: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                    return false;
                }
            }
            return false;
        }

        private void DeleteResource(Package package, TGI tgiToDelete)
        {
            IResourceKey key = new TGIBlock(1, null, tgiToDelete.Type, tgiToDelete.Group, tgiToDelete.Instance);
            DeleteResource(package, key);
        }
        private void DeleteResource(Package package, IResourceIndexEntry keyToDelete)
        {
            DeleteResource(package, (IResourceKey)keyToDelete);
        }
        private void DeleteResource(Package package, IResourceKey keyToDelete)
        {
            Predicate<IResourceIndexEntry> idel = r => r.ResourceType == keyToDelete.ResourceType &
                    r.ResourceGroup == keyToDelete.ResourceGroup & r.Instance == keyToDelete.Instance;
            List<IResourceIndexEntry> iries = package.FindAll(idel);
            foreach (IResourceIndexEntry irie in iries)
            {
                package.DeleteResource(irie);
            }
            iries = package.FindAll(idel);
            if (iries.Count > 0) MessageBox.Show("DeleteResource didn't work correctly! " + iries.Count.ToString() + " are left.");
            foreach (IResourceIndexEntry irie in iries)
            {
                package.DeleteResource(irie);
            }
            iries = package.FindAll(idel);
            if (iries.Count > 0) MessageBox.Show("DeleteResource didn't work correctly! " + iries.Count.ToString() + " are still left.");
        }

        private void TextureOnly_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetShineControl();
        }

        private void CASPs_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetShineControl();
        }

        private void SetShineControl()
        {
            Shine_checkBox.Enabled = CASPs_radioButton.Checked;
        }
    }
}
