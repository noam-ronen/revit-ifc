﻿//
// BIM IFC library: this library works with Autodesk(R) Revit(R) to export IFC files containing model geometry.
// Copyright (C) 2012  Autodesk, Inc.
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.IFC;
using Revit.IFC.Export.Utility;
using Revit.IFC.Common.Utility;

namespace Revit.IFC.Export.Exporter.PropertySet.Calculators
{
   /// <summary>
   /// A calculation class to calculate cross section area.
   /// </summary>
   class CrossSectionAreaCalculator : PropertyCalculator
   {
      /// <summary>
      /// A double variable to keep the calculated value.
      /// </summary>
      private double m_Area = 0;

      /// <summary>
      /// A static instance of this class.
      /// </summary>
      static CrossSectionAreaCalculator s_Instance = new CrossSectionAreaCalculator();

      /// <summary>
      /// The CrossSectionAreaCalculator instance.
      /// </summary>
      public static CrossSectionAreaCalculator Instance
      {
         get { return s_Instance; }
      }

      /// <summary>
      /// Calculates cross area.
      /// </summary>
      /// <param name="exporterIFC">
      /// The ExporterIFC object.
      /// </param>
      /// <param name="extrusionCreationData">
      /// The IFCExtrusionCreationData.
      /// </param>
      /// <param name="element">
      /// The element to calculate the value.
      /// </param>
      /// <param name="elementType">
      /// The element type.
      /// </param>
      /// <returns>
      /// True if the operation succeed, false otherwise.
      /// </returns>
      public override bool Calculate(ExporterIFC exporterIFC, IFCExtrusionCreationData extrusionCreationData, Element element, ElementType elementType, EntryMap entryMap)
      {
         double crossSectionArea = 0;

         // 1. Check override parameter
         if (ParameterUtil.GetDoubleValueFromElementOrSymbol(element, entryMap.RevitParameterName, out crossSectionArea) == null)
            if (ParameterUtil.GetDoubleValueFromElementOrSymbol(element, entryMap.CompatibleRevitParameterName, out crossSectionArea) == null)
               ParameterUtil.GetDoubleValueFromElementOrSymbol(element, "IfcQtyCrossSectionArea", out crossSectionArea);
         m_Area = UnitUtil.ScaleArea(crossSectionArea);
         if (m_Area > MathUtil.Eps() * MathUtil.Eps())
            return true;

         // 2. try using extrusion data
         if (extrusionCreationData != null)
         {
            m_Area = extrusionCreationData.ScaledArea;
            if (m_Area > MathUtil.Eps() * MathUtil.Eps())
               return true;
         }

         // 3. Use the builtin parameter after extrusionCreationData because the HOST_AREA_COMPUTED returns total area
         if (ParameterUtil.GetDoubleValueFromElement(element, BuiltInParameter.HOST_AREA_COMPUTED, out crossSectionArea) != null)
         {
            m_Area = UnitUtil.ScaleArea(crossSectionArea);
            if (m_Area > MathUtil.Eps() * MathUtil.Eps())
               return true;
         }
         return false;
      }

      /// <summary>
      /// Gets the calculated double value.
      /// </summary>
      /// <returns>
      /// The double value.
      /// </returns>
      public override double GetDoubleValue()
      {
         return m_Area;
      }
   }
}
