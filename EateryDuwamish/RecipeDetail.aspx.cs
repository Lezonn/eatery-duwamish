using BusinessFacade;
using Common.Data;
using Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EateryDuwamish
{
    public partial class RecipeDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowNotificationIfExists();
                if (!string.IsNullOrEmpty(Request.QueryString["ID"]))
                {
                    GetRecipeByID(Convert.ToInt32(Request.QueryString["ID"]));
                    LoadIngredientTable(Convert.ToInt32(Request.QueryString["ID"]));
                }
                else
                {
                    Response.Redirect("Dish.aspx");
                }
            }
        }

        #region GET RECIPE NAME AND DESCRIPTION
        private void GetRecipeByID(int ID)
        {
            RecipeData Recipe = new RecipeSystem().GetRecipeByID(ID);
            lblRecipeName.Text = Recipe.RecipeName;
            if(Recipe.RecipeDescription != "")
            {
                txtRecipeDescription.Text = Recipe.RecipeDescription;
            }
        }
        #endregion

        #region FORM MANAGEMENT
        private void FillForm(IngredientData ingredient)
        {
            hdfIngredientId.Value = ingredient.IngredientID.ToString();
            txtIngredientName.Text = ingredient.IngredientName;
            txtQuantity.Text = ingredient.Quantity.ToString();
            txtUnit.Text = ingredient.Unit;
        }

        private void ResetForm()
        {
            hdfIngredientId.Value = String.Empty;
            txtIngredientName.Text = String.Empty;
            txtQuantity.Text = String.Empty;
            txtUnit.Text = String.Empty;
        }

        private IngredientData GetFormData()
        {
            IngredientData ingredient = new IngredientData();
            ingredient.IngredientID = String.IsNullOrEmpty(hdfIngredientId.Value) ? 0 : Convert.ToInt32(hdfIngredientId.Value);
            ingredient.RecipeID = Convert.ToInt32(Request.QueryString["ID"]);
            ingredient.IngredientName = txtIngredientName.Text;
            ingredient.Quantity = Convert.ToInt32(txtQuantity.Text);
            ingredient.Unit = txtUnit.Text;

            return ingredient;
        }

        private RecipeData GetFormDescription()
        {
            RecipeData description = new RecipeData();
            description.RecipeID = Convert.ToInt32(Request.QueryString["ID"]);
            description.RecipeDescription = Convert.ToString(txtRecipeDescription.Text);

            return description;
        }

        #endregion

        #region DATA TABLE MANAGEMENT
        private void LoadIngredientTable(int recipeID)
        {
            try
            {
                List<IngredientData> ListIngredient = new RecipeDetailSystem().GetIngredientList(recipeID);
                rptIngredient.DataSource = ListIngredient;
                rptIngredient.DataBind();
            }
            catch (Exception ex)
            {
                notifRecipe.Show($"ERROR LOAD TABLE: {ex.Message}", NotificationType.Danger);
            }
        }
        protected void rptIngredient_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                IngredientData ingredient = (IngredientData)e.Item.DataItem;

                Literal litIngredientName = (Literal)e.Item.FindControl("litIngredientName");
                Literal litQuantity = (Literal)e.Item.FindControl("litQuantity");
                Literal litUnit = (Literal)e.Item.FindControl("litUnit");
                LinkButton lbIngredientEdit = (LinkButton)e.Item.FindControl("lbIngredientEdit");

                litIngredientName.Text = ingredient.IngredientName;
                litQuantity.Text = ingredient.Quantity.ToString();
                litUnit.Text = ingredient.Unit;

                lbIngredientEdit.Text = "Edit";
                lbIngredientEdit.CommandArgument = ingredient.IngredientID.ToString();

                CheckBox chkChoose = (CheckBox)e.Item.FindControl("chkChoose");
                chkChoose.Attributes.Add("data-value", ingredient.IngredientID.ToString());
            }
        }
        protected void rptIngredient_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EDIT")
            {
                Literal litIngredientName = (Literal)e.Item.FindControl("litIngredientName");
                Literal litQuantity = (Literal)e.Item.FindControl("litQuantity");
                Literal litUnit = (Literal)e.Item.FindControl("litUnit");
                

                int ingredientID = Convert.ToInt32(e.CommandArgument.ToString());
                IngredientData ingredient = new RecipeDetailSystem().GetIngredientByID(ingredientID);
                FillForm(new IngredientData
                {
                    IngredientID = ingredient.IngredientID,
                    IngredientName = ingredient.IngredientName,
                    Quantity = ingredient.Quantity,
                    Unit = ingredient.Unit
                });
                litFormType.Text = $"UBAH: {litIngredientName.Text}";
                pnlFormIngredient.Visible = true;
                txtIngredientName.Focus();
            }
        }
        #endregion

        #region BUTTON EVENT MANAGEMENT
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                IngredientData ingredient = GetFormData();
                int rowAffected = new RecipeDetailSystem().InsertUpdateIngredient(ingredient);
                if (rowAffected <= 0)
                    throw new Exception("No Data Recorded");
                Session["save-success"] = 1;
                Response.Redirect("RecipeDetail.aspx?ID=" + Convert.ToInt32(Request.QueryString["ID"]));
            }
            catch (Exception ex)
            {
                notifRecipe.Show($"ERROR SAVE DATA: {ex.Message}", NotificationType.Danger);
            }
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ResetForm();
            litFormType.Text = "TAMBAH";
            pnlFormIngredient.Visible = true;
            txtIngredientName.Focus();
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string strDeletedIDs = hdfDeletedIngredients.Value;
                IEnumerable<int> deletedIDs = strDeletedIDs.Split(',').Select(Int32.Parse);
                int rowAffected = new RecipeDetailSystem().DeleteIngredients(deletedIDs);
                if (rowAffected <= 0)
                    throw new Exception("No Data Deleted");
                Session["delete-success"] = 1;
                Response.Redirect("RecipeDetail.aspx?ID=" + Convert.ToInt32(Request.QueryString["ID"]));
            }
            catch (Exception ex)
            {
                notifRecipe.Show($"ERROR DELETE DATA: {ex.Message}", NotificationType.Danger);
            }
        }
        protected void btnEditDesc_Click(object sender, EventArgs e)
        {
            txtRecipeDescription.ReadOnly = false;
            txtRecipeDescription.Focus();
        }
        protected void btnSaveDesc_Click(object sender, EventArgs e)
        {
            try
            {
                RecipeData description = GetFormDescription();
                int rowAffected = new RecipeDetailSystem().InsertUpdateDescription(description);
                if (rowAffected <= 0)
                    throw new Exception("No Data Recorded");
                Session["save-success"] = 1;
                Response.Redirect("RecipeDetail.aspx?ID=" + Convert.ToInt32(Request.QueryString["ID"]));
            }
            catch (Exception ex)
            {
                notifRecipe.Show($"ERROR SAVE DATA: {ex.Message}", NotificationType.Danger);
            }
            txtRecipeDescription.ReadOnly = true;
        }
        #endregion

        #region NOTIFICATION MANAGEMENT
        private void ShowNotificationIfExists()
        {
            if (Session["save-success"] != null)
            {
                notifRecipe.Show("Data sukses disimpan", NotificationType.Success);
                Session.Remove("save-success");
            }
            if (Session["delete-success"] != null)
            {
                notifRecipe.Show("Data sukses dihapus", NotificationType.Success);
                Session.Remove("delete-success");
            }
        }
        #endregion
    }
}