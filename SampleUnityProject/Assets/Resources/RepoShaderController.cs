/**
*  Copyright (C) 2018 3D Repo Ltd
*
*  This program is free software: you can redistribute it and/or modify
*  it under the terms of the GNU Affero General Public License as
*  published by the Free Software Foundation, either version 3 of the
*  License, or (at your option) any later version.
*
*  This program is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU Affero General Public License for more details.
*
*  You should have received a copy of the GNU Affero General Public License
*  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using UnityEngine;

public class RepoShaderController : MonoBehaviour
{        
    public int width, height;
    /**
    * Set clipping controller
    * @param ctrl ClipController to link up with
    */
    public void SetTextureDimension(
        int w,
        int h)
    {
        width = w;
        height = h;
        UpdateShaderProperties();
    }

    public void HideObject(int index) {
        foreach (Material m in GetComponent<Renderer>().sharedMaterials)
        {
            m.SetInt("_HiddenObject", index);
        }
    }

    public void UnhideObject()
    {
        foreach (Material m in GetComponent<Renderer>().sharedMaterials)
        {
            m.SetInt("_HiddenObject", -1);
        }
    }

    /**
    * Update shader properties
    */
    private void UpdateShaderProperties()
    {
        foreach (Material m in GetComponent<Renderer>().sharedMaterials)
        {
            m.SetFloat("_MapHeight", height);
            m.SetFloat("_MapWidth", width);
            m.SetInt("_HiddenObject", -1);
        }

    }
        
}
