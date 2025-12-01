using UnityEngine;
using System;
using System.Reflection;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Gets the value from the field specified. Returns success if the field was retrieved.")]
    [TaskCategory("Reflection")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class MyGetFieldValue : Action
    {
        [Tooltip("The GameObject to get the field on")]
        public SharedGameObject targetGameObject;
        [Tooltip("The component to get the field on")]
        public SharedString componentName;
        [Tooltip("The name of the field")]
        public SharedString fieldName;
        [Tooltip("The value of the field")]
        [RequiredField]
        public SharedVariable fieldValue;
        [Tooltip("Whether to reevaluate the field value on each update")]
        public bool reevaluate = false;

        private FieldInfo fieldInfo;
        private Component component;

        public override void OnStart()
        {
            // 获取组件和字段信息
            var type = TaskUtility.GetTypeWithinAssembly(componentName.Value);
            if (type == null)
            {
                Debug.LogWarning("Unable to get field - type is null");
                return;
            }

            component = GetDefaultGameObject(targetGameObject.Value).GetComponent(type);
            if (component == null)
            {
                Debug.LogWarning("Unable to get the field with component " + componentName.Value);
                return;
            }

            fieldInfo = component.GetType().GetField(fieldName.Value);
        }

        public override TaskStatus OnUpdate()
        {
            if (fieldInfo == null)
            {
                Debug.LogWarning("Unable to get field - field info is null");
                return TaskStatus.Failure;
            }

            if (reevaluate)
            {
                fieldValue.SetValue(fieldInfo.GetValue(component));
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            componentName = null;
            fieldName = null;
            fieldValue = null;
            reevaluate = false;
            fieldInfo = null;
            component = null;
        }
    }
}
