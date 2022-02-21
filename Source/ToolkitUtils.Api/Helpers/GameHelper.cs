﻿// ToolkitUtils.Core
// Copyright (C) 2021  SirRandoo
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class GameHelper
    {
        // https://stackoverflow.com/a/42246387
        [NotNull]
        public static IEnumerable<Type> GetAllTypes([NotNull] Type genericType, params Type[] genericParameters)
        {
            if (!genericType.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Specified type must be a generic type definition", nameof(genericType));
            }

            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => IsGenericType(t, genericType, false, genericParameters));
        }

        public static bool IsGenericTypeDeep([NotNull] Type type, Type genericType, bool fuzzy = false, params Type[] genericParams)
        {
            if (IsGenericType(type, genericType, fuzzy, genericParams))
            {
                return true;
            }

            foreach (Type @interface in type.GetInterfaces())
            {
                if (IsGenericTypeDeep(@interface, genericType, fuzzy, genericParams))
                {
                    return true;
                }
            }

            return type.BaseType != null && IsGenericTypeDeep(type.BaseType, genericType, fuzzy, genericParams);
        }

        public static bool IsGenericType([NotNull] Type type, Type genericType, bool fuzzy = false, params Type[] genericParams)
        {
            if (type.IsGenericType && genericType.IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                return true;
            }

            if (type.GetInterfaces().Any(i => i.IsGenericType && genericType.IsAssignableFrom(i.GetGenericTypeDefinition())))
            {
                return true;
            }

            if (fuzzy)
            {
                return false;
            }

            Type[] args = type.GetGenericArguments();

            return args.Length == genericParams.Length && args.Zip(genericParams, (f, s) => s.IsAssignableFrom(f)).All(c => c);
        }

        public static bool GetDefaultUsability([NotNull] ThingDef thing)
        {
            if (thing.tradeTags.NullOrEmpty())
            {
                return true;
            }

            foreach (string tag in thing.tradeTags)
            {
                if (tag.Equals("Artifact", StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }

                if (tag.Equals("ExoticMisc", StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool GetDefaultMaterialState([NotNull] ThingDef thing)
        {
            if (!thing.IsStuff)
            {
                return false;
            }

            var rarity = 1f;
            float commonality = thing.stuffProps.commonality;

            if (commonality < 1.0f)
            {
                return false;
            }

            if (thing.researchPrerequisites.NullOrEmpty())
            {
                return rarity >= 0.85f;
            }

            foreach (ResearchProjectDef project in thing.researchPrerequisites)
            {
                var tier = (int)project.techLevel;

                if (tier <= 1)
                {
                    continue;
                }

                rarity *= (float)(int)TechLevel.Neolithic / (int)project.techLevel;

                if (project.TechprintCount > 0)
                {
                    rarity *= project.TechprintCount / 25f * project.techprintCommonality;
                }
            }

            return rarity >= 0.85f;
        }

        [NotNull]
        public static IEnumerable<Type> GetAllTypes([NotNull] Type @interface)
        {
            if (!@interface.IsInterface)
            {
                throw new ArgumentException("Specified type must be an interface definition", nameof(@interface));
            }

            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsClass && @interface.IsAssignableFrom(t));
        }
    }
}
