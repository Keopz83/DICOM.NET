﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joselima.dicom.viewer.wpf.ViewModels {

    public class DataSetViewModel {

        public ObservableCollection<AttributeViewModel> Items { get; set; }
            = new ObservableCollection<AttributeViewModel>();

        internal void Load(IEnumerable<string> fileAbsolutePaths) {

            try {
                var dicomFile = new FileParser().Parse(fileAbsolutePaths.First(), 0x7FE0000F);
                Items.Clear();
                var newItems = dicomFile.Attributes.Select(x =>
                        new AttributeViewModel() {
                            Tag = x.Value.Tag.ToString(),
                            Value = x.Value.Value.ToString().Trim(),
                        });
                foreach (var item in newItems) {
                    Items.Add(item);
                }
            } catch(Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }

    public class AttributeViewModel {

        public string Tag { get; set; }
        public string Value { get; set; }
    }
}
