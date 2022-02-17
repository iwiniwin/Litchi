using System;
using System.Collections.Generic;

namespace Litchi
{
    public interface IComponentExtend
    {
        List<Component> boundComponents {get;}
    }

    public static class ComponentExtensions
    {
        public static T BindComponent<T>(this IComponentExtend extend) where T : Component, new()
        {
            return bindComponent(extend, new T()) as T;
        }

        public static Component BindComponent(this IComponentExtend extend, string className)
        {
            return extend.BindComponent(Type.GetType(className));
        }

        public static Component BindComponent(this IComponentExtend extend, Type componentType)
        {
            if(!componentType.IsSubclassOf(typeof(Component))){
                throw new Exception("component type error");
            }
            return bindComponent(extend, componentType.Assembly.CreateInstance(componentType.FullName) as Component);
        }

        public static Component UnbindComponent<T>(this IComponentExtend extend) where T : Component
        {
            return null;
        }

        public static Component UnbindComponent(this IComponentExtend extend, string className)
        {
            return null;
        }

        public static Component UnbindComponent(this IComponentExtend extend, Type componentType)
        {
            return null;
        }

        public static T GetComponent<T>(this IComponentExtend extend) where T : Component
        {
            return getComponent(extend, typeof(T)) as T;
        }

        public static Component GetComponent(this IComponentExtend extend, Type componentType)
        {
            return getComponent(extend, componentType);
        }

        public static Component GetComponent(this IComponentExtend extend, string className)
        {
            return getComponent(extend, Type.GetType(className));
        }

        public static Component[] GetComponents(this IComponentExtend extend, Type componentType)
        {
            return null;
        }

        public static T[] GetComponents<T>(this IComponentExtend extend)
        {
            return null;
        }

        public static void GetComponents(this IComponentExtend extend, Type type, List<Component> results)
        {
            
        }

        public static void GetComponents<T>(this IComponentExtend extend, List<T> results)
        {
            
        }

        private static Component bindComponent(IComponentExtend extend, Component component)
        {
            checkBoundComponentsInit(extend);
            extend.boundComponents.Add(component);
            component.OnBind();
            return component;
        }

        private static Component getComponent(IComponentExtend extend, Type componentType)
        {
            checkBoundComponentsInit(extend);
            foreach (var item in extend.boundComponents)
            {
                if(componentType == item.GetType())
                {
                    return item;
                }
            }
            return null;
        }

        private static void checkBoundComponentsInit(IComponentExtend extend)
        {
            if(extend.boundComponents == null || extend.boundComponents != extend.boundComponents)
            {
                throw new Exception(extend.GetType().FullName + ".boundComponents init failed");
            }
        }
    }
}