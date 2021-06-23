using BusinessRule;
using Common.Data;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessFacade
{
    public class RecipeDetailSystem
    {
        public List<IngredientData> GetIngredientList(int RecipeID)
        {
            try
            {
                return new RecipeDetailDB().GetIngredientList(RecipeID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IngredientData GetIngredientByID(int ingredientID)
        {
            try
            {
                return new RecipeDetailDB().GetIngredientByID(ingredientID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertUpdateIngredient(IngredientData ingredient)
        {
            try
            {
                return new RecipeDetailRule().InsertUpdateIngredient(ingredient);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int DeleteIngredients(IEnumerable<int> ingredientIDs)
        {
            try
            {
                return new RecipeDetailRule().DeleteIngredients(ingredientIDs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertUpdateDescription(RecipeData description)
        {
            try
            {
                return new RecipeDetailRule().InsertUpdateDescription(description);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
