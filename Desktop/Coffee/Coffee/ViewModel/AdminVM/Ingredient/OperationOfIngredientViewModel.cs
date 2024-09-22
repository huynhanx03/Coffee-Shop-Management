using Coffee.DTOs;
using Coffee.Services;
using Coffee.Utils;
using Coffee.Views.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Ingredient
{
    public partial class IngredientViewModel: BaseViewModel
    {

        #region variable
        public string _IngredientName;
        public string IngredientName
        { 
            get {  return _IngredientName; }
            set { _IngredientName = value; OnPropertyChanged(); }
        }

        public string _SelectedUnitName;
        public string SelectedUnitName
        {
            get { return _SelectedUnitName; }
            set { _SelectedUnitName = value; OnPropertyChanged(); }
        }

        private int TypeOperation;

        #endregion

        #region ICommand
        public ICommand closeOperationIngredientWindowIC {  get; set; }
        public ICommand confirmOperationIngredientIC {  get; set; }
        #endregion

        /// <summary>
        /// Xác nhận thao tác nguyên liệu
        /// </summary>
        private async void confirmOperationIngredient()
        {
            UnitDTO Unit = (UnitList.First(p => p.TenDonVi == SelectedUnitName) as UnitDTO);

            IngredientDTO ingredient = new IngredientDTO
            {
                TenNguyenLieu = IngredientName.Trim(),
                MaDonVi = Unit.MaDonVi,
            };

            switch (TypeOperation)
            {
                case 1:
                    (string label, IngredientDTO NewIngredient) = await IngredientService.Ins.createIngredient(ingredient);

                    if (NewIngredient != null)
                    {
                        MessageBoxCF ms = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                        ms.ShowDialog();
                        resetIngredient();

                        loadIngredientList();
                    }
                    else
                    {
                        MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                        ms.ShowDialog();
                    }
                    break;
                case 2:
                    ingredient.MaNguyenLieu = SelectedIngredient.MaNguyenLieu;
                    ingredient.SoLuong = SelectedIngredient.SoLuong;

                    (string labelEdit, IngredientDTO NewProductEdit) = await IngredientService.Ins.updateIngredient(ingredient);

                    if (NewProductEdit != null)
                    {
                        MessageBoxCF ms = new MessageBoxCF(labelEdit, MessageType.Accept, MessageButtons.OK);
                        ms.ShowDialog();
                        loadIngredientList();
                    }
                    else
                    {
                        MessageBoxCF ms = new MessageBoxCF(labelEdit, MessageType.Error, MessageButtons.OK);
                        ms.ShowDialog();
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
