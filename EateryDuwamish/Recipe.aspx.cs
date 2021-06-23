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
    public partial class Recipe : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowNotificationIfExists();
                if (!string.IsNullOrEmpty(Request.QueryString["ID"]))
                {
                    GetDishName(Convert.ToInt32(Request.QueryString["ID"]));
                    LoadRecipeTable(Convert.ToInt32(Request.QueryString["ID"]));
                }
                else
                {
                    Response.Redirect("Dish.aspx");
                }
            }
        }

        #region DISH NAME
        private void GetDishName(int ID)
        {
            DishData Dish = new DishSystem().GetDishByID(ID);
            lblDishName.Text = Dish.DishName;
        }
        #endregion
    
        #region FORM MANAGEMENT
        private void ResetForm()
        {
            hdfRecipeId.Value = String.Empty;
            txtRecipeName.Text = String.Empty;
        }
        private RecipeData GetFormData()
        {
            RecipeData recipe = new RecipeData();
            recipe.RecipeID = String.IsNullOrEmpty(hdfRecipeId.Value) ? 0 : Convert.ToInt32(hdfRecipeId.Value);
            recipe.DishID = Convert.ToInt32(Request.QueryString["ID"]);
            recipe.RecipeName = txtRecipeName.Text;
            return recipe;
        }

        #endregion

        #region DATA TABLE MANAGEMENT
        private void LoadRecipeTable(int dishID)
        {
            try
            {
                List<RecipeData> ListRecipe = new RecipeSystem().GetRecipeList(dishID);
                rptRecipe.DataSource = ListRecipe;
                rptRecipe.DataBind();
            }
            catch (Exception ex)
            {
                notifRecipe.Show($"ERROR LOAD TABLE: {ex.Message}", NotificationType.Danger);
            }
        }
        protected void rptRecipe_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RecipeData recipe = (RecipeData)e.Item.DataItem;
                Literal litRecipeName = (Literal)e.Item.FindControl("litRecipeName");
                LinkButton lbRecipeDetail = (LinkButton)e.Item.FindControl("lbRecipeDetail");

                litRecipeName.Text = recipe.RecipeName;

                lbRecipeDetail.Text = "Detail";
                lbRecipeDetail.CommandArgument = recipe.RecipeID.ToString();

                CheckBox chkChoose = (CheckBox)e.Item.FindControl("chkChoose");
                chkChoose.Attributes.Add("data-value", recipe.RecipeID.ToString());
            }
        }
        protected void rptRecipe_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DETAIL")
            {
                int recipeID = Convert.ToInt32(e.CommandArgument.ToString());
                Response.Redirect("RecipeDetail.aspx?ID=" + recipeID);
            }
        }
        #endregion

        #region BUTTON EVENT MANAGEMENT
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                RecipeData recipe = GetFormData();
                int rowAffected = new RecipeSystem().InsertRecipe(recipe);
                if (rowAffected <= 0)
                    throw new Exception("No Data Recorded");
                Session["save-success"] = 1;
                Response.Redirect("Recipe.aspx?ID=" + Convert.ToInt32(Request.QueryString["ID"]));
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
            pnlFormRecipe.Visible = true;
            txtRecipeName.Focus();
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string strDeletedIDs = hdfDeletedRecipes.Value;
                IEnumerable<int> deletedIDs = strDeletedIDs.Split(',').Select(Int32.Parse);
                int rowAffected = new RecipeSystem().DeleteRecipes(deletedIDs);
                if (rowAffected <= 0)
                    throw new Exception("No Data Deleted");
                Session["delete-success"] = 1;
                Response.Redirect("Recipe.aspx?ID=" + Convert.ToInt32(Request.QueryString["ID"]));
            }
            catch (Exception ex)
            {
                notifRecipe.Show($"ERROR DELETE DATA: {ex.Message}", NotificationType.Danger);
            }
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