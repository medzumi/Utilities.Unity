using UnityEngine;
using UnityEngine.UIElements;

namespace medzumi.utilities.unity.VisualElements
{
    public class Foldout : BindableElement
    {
        private Toggle m_Toggle;
        private VisualElement m_Container;
        private VisualElement m_HeaderContent;
        private bool m_Value;

        /// <summary>
        ///        <para>
        /// The USS class name for Foldout elements.
        /// </para>
        ///      </summary>
        public static readonly string ussClassName = "unity-foldout";

        /// <summary>
        ///        <para>
        /// The USS class name of Toggle sub-elements in Foldout elements.
        /// </para>
        ///      </summary>
        public static readonly string toggleUssClassName = Foldout.ussClassName + "__toggle";

        /// <summary>
        ///        <para>
        /// The USS class name for the content element in a Foldout.
        /// </para>
        ///      </summary>
        public static readonly string contentUssClassName = Foldout.ussClassName + "__content";

        /// <summary>
        ///        <para>
        /// The USS class name for the Label element in a Foldout.
        /// </para>
        ///      </summary>
        public static readonly string inputUssClassName = Foldout.ussClassName + "__input";

        /// <summary>
        ///        <para>
        /// The USS class name for the Label element in a Foldout.
        /// </para>
        ///      </summary>
        public static readonly string checkmarkUssClassName =
            Foldout.ussClassName + "__checkmark";

        /// <summary>
        ///        <para>
        /// The USS class name for the Label element in a Foldout.
        /// </para>
        ///      </summary>
        public static readonly string textUssClassName = UnityEngine.UIElements.Foldout.ussClassName + "__text";

        internal static readonly string toggleInspectorUssClassName =
            UnityEngine.UIElements.Foldout.toggleUssClassName + "--inspector";

        internal static readonly string ussFoldoutDepthClassName =
            UnityEngine.UIElements.Foldout.ussClassName + "--depth-";

        internal static readonly int ussFoldoutMaxDepth = 4;

        internal Toggle toggle => this.m_Toggle;

        /// <summary>
        ///        <para>
        /// This element contains the elements that are shown or hidden when you toggle the Foldout.
        /// </para>
        ///      </summary>
        public override VisualElement contentContainer => this.m_Container;

        /// <summary>
        ///        <para>
        /// This is the text of the toggle's label.
        /// </para>
        ///      </summary>
        public string text
        {
            get => this.m_Toggle.text;
            set
            {
                this.m_Toggle.text = value;
                this.m_Toggle.Q((string)null, Toggle.textUssClassName)
                    ?.AddToClassList(UnityEngine.UIElements.Foldout.textUssClassName);
            }
        }

        /// <summary>
        ///        <para>
        /// This is the state of the Foldout's toggle. It is true if the Foldout is open and its contents are
        /// visible, and false if the Foldout is closed, and its contents are hidden.
        /// </para>
        ///      </summary>
        public bool value
        {
            get => this.m_Value;
            set
            {
                if (this.m_Value == value)
                    return;
                using (ChangeEvent<bool> pooled = ChangeEvent<bool>.GetPooled(this.m_Value, value))
                {
                    this.SetValueWithoutNotify(value);
                    this.SendEvent((EventBase)pooled);
                }
            }
        }

        /// <summary>
        ///        <para>
        /// Sets the value of the Foldout's Toggle sub-element, but does not notify the rest of the hierarchy of the change.
        /// </para>
        ///      </summary>
        /// <param name="newValue">The new value of the foldout</param>
        public void SetValueWithoutNotify(bool newValue)
        {
            this.m_Value = newValue;
            this.m_Toggle.SetValueWithoutNotify(this.m_Value);
            this.contentContainer.style.display =
                (StyleEnum<DisplayStyle>)(newValue ? DisplayStyle.Flex : DisplayStyle.None);
        }

        /// <summary>
        ///        <para>
        /// Constructs a Foldout element.
        /// </para>
        ///      </summary>
        public Foldout()
        {
            this.AddToClassList(UnityEngine.UIElements.Foldout.ussClassName);
            this.m_Toggle = new Toggle();
            this.m_HeaderContent = new VisualElement()
            {
                name = "unity-content"
            };
            this.m_Container = new VisualElement()
            {
                name = "unity-content"
            };
            this.m_Toggle.RegisterValueChangedCallback<bool>((EventCallback<ChangeEvent<bool>>)(evt =>
            {
                this.value = this.m_Toggle.value;
                evt.StopPropagation();
            }));
            this.m_Toggle.AddToClassList(Foldout.toggleUssClassName);
            this.m_Toggle.AddToClassList(Foldout.inputUssClassName);
            this.m_Toggle.Q((string)null, Toggle.checkmarkUssClassName)
                .AddToClassList(Foldout.checkmarkUssClassName);
            this.hierarchy.Add(m_HeaderContent);
            m_HeaderContent.hierarchy.Add((VisualElement)this.m_Toggle);
            this.m_Container.AddToClassList(UnityEngine.UIElements.Foldout.contentUssClassName);
            this.hierarchy.Add(this.m_Container);
            this.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel));
            this.SetValueWithoutNotify(true);
        }

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            for (int index = 0; index <= Foldout.ussFoldoutMaxDepth; ++index)
                this.RemoveFromClassList(Foldout.ussFoldoutDepthClassName + index.ToString());
            this.RemoveFromClassList(Foldout.ussFoldoutDepthClassName + "max");
        }

        /// <summary>
        ///        <para>
        /// Instantiates a Foldout using the data from a UXML file.
        /// </para>
        ///      </summary>
        public new class UxmlFactory : UnityEngine.UIElements.UxmlFactory<Foldout,
            Foldout.UxmlTraits>
        {
        }

        /// <summary>
        ///        <para>
        /// Defines UxmlTraits for the Foldout.
        /// </para>
        ///      </summary>
        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            private UxmlStringAttributeDescription m_Text;
            private UxmlBoolAttributeDescription m_Value;

            /// <summary>
            ///        <para>
            /// Initializes Foldout properties using values from the attribute bag.
            /// </para>
            ///      </summary>
            /// <param name="ve">The object to initialize.</param>
            /// <param name="bag">The attribute bag.</param>
            /// <param name="cc">The creation context; unused.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (!(ve is UnityEngine.UIElements.Foldout foldout))
                    return;
                foldout.text = this.m_Text.GetValueFromBag(bag, cc);
                foldout.SetValueWithoutNotify(this.m_Value.GetValueFromBag(bag, cc));
            }

            public UxmlTraits()
            {
                UxmlStringAttributeDescription attributeDescription1 = new UxmlStringAttributeDescription();
                attributeDescription1.name = "text";
                this.m_Text = attributeDescription1;
                UxmlBoolAttributeDescription attributeDescription2 = new UxmlBoolAttributeDescription();
                attributeDescription2.name = "value";
                attributeDescription2.defaultValue = true;
                this.m_Value = attributeDescription2;
                // ISSUE: explicit constructor call
            }
        }
    }
}