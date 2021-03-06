//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
namespace System.Activities.Presentation.Internal.PropertyEditing.State 
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    // <summary>
    // Simple wrapper around a dictionary of CategoryStates keyed by the category names.
    // </summary>
    internal class CategoryStateContainer : PersistedStateContainer 
    {

        // <summary>
        // Gets the CategoryState for the specified category.  If one does not exist
        // yet, it will be created automatically, guaranteeing a non-null return value.
        // </summary>
        // <param name="categoryName">Name of the requested category</param>
        // <returns>A non-null instance of CategoryState</returns>
        public CategoryState GetCategoryState(string categoryName) 
        {
            return (CategoryState)this.GetState(categoryName);
        }

        // <summary>
        // Creates a default state object based on the specified key
        // </summary>
        // <param name="key">Key of the state object</param>
        // <returns>Default state object</returns>
        protected override PersistedState CreateDefaultState(object key) 
        {
            return new CategoryState(key as string);
        }

        // <summary>
        // Deserializes the specified string value into a state object
        // </summary>
        // <param name="serializedValue">Serialized value of the state object</param>
        // <returns>Deserialized instance of the state object</returns>
        protected override PersistedState DeserializeState(string serializedValue) 
        {
            return CategoryState.Deserialize(serializedValue);
        }
    }
}
