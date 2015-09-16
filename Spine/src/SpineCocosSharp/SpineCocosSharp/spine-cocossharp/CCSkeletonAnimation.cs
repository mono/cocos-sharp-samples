using Spine;


namespace CocosSharp.Spine
{


	public class CCSkeletonAnimation : CCSkeleton
	{


		public delegate void StartEndDelegate (AnimationState state, int trackIndex);

		public delegate void EventDelegate (AnimationState state, int trackIndex, Event e);

		public delegate void CompleteDelegate (AnimationState state, int trackIndex, int loopCount);

		public float TimeScale = 1;

		public event StartEndDelegate Start;
		public event StartEndDelegate End;
		public event EventDelegate Event;
		public event CompleteDelegate Complete;

		AnimationState State { get; set; }

		public bool OwnsAnimationStateData { get; private set; }


		#region Constructors


		public CCSkeletonAnimation ()
		{
			Initializer ();
		}


		public CCSkeletonAnimation (SkeletonData skeletonData) : base (skeletonData)
		{
			Initializer ();
		}


		public CCSkeletonAnimation (string skeletonDataFile, Atlas atlas, float scale) : base (skeletonDataFile, atlas, scale)
		{
			Initializer ();
		}


		public CCSkeletonAnimation (string skeletonDataFile, string atlasFile, float scale) : base (skeletonDataFile, atlasFile, scale)
		{
			Initializer ();
		}


		#endregion


		#region Public

        
		public void SetAnimationStateData (AnimationStateData stateData)
		{
			OwnsAnimationStateData &= stateData == null;
			
			State = new AnimationState (stateData);

			State.Event += OnEvent;
			State.Start += OnStart;
			State.Complete += OnComplete;
			State.End += OnEnd;
		}


		public override void Update (float dt)
		{
			base.Update (dt);

			dt *= TimeScale;

			State.Update (dt);
			State.Apply (Skeleton);

			UpdateWorldTransform ();
		}


		public void SetMix (string fromAnimation, string toAnimation, float duration)
		{
			State.Data.SetMix (fromAnimation, toAnimation, duration);
		}


		public TrackEntry SetAnimation (int trackIndex, string name, bool loop)
		{
			var animation = Skeleton.Data.FindAnimation (name);

			if (animation == null) {
				CCLog.Log (string.Format ("Spine: Animation not found: '{0}'", name));
				return null;
			}

			return State.SetAnimation (trackIndex, animation, loop);
		}


		public TrackEntry AddAnimation (int trackIndex, string name, bool loop, float delay = 0)
		{
			var animation = Skeleton.Data.FindAnimation (name);

			if (animation == null) {
				CCLog.Log ("Spine: Animation not found: '{0}'", name);
				return null;
			}

			return State.AddAnimation (trackIndex, animation, loop, delay);
		}


		#endregion


		#region Private


		void Initializer ()
		{
			OwnsAnimationStateData = true;

			State = new AnimationState (new AnimationStateData (Skeleton.Data));
			State.Event += OnEvent;
			State.Start += OnStart;
			State.Complete += OnComplete;
			State.End += OnEnd;
		}


		void OnEnd (AnimationState state, int trackIndex)
		{
			if (End != null)
				End (state, trackIndex);
		}


		void OnComplete (AnimationState state, int trackIndex, int loopCount)
		{
			if (Complete != null)
				Complete (state, trackIndex, loopCount);
		}


		void OnStart (AnimationState state, int trackIndex)
		{
			if (Start != null)
				Start (state, trackIndex);
		}


		void OnEvent (AnimationState state, int trackIndex, Event e)
		{
			if (Event != null)
				Event (state, trackIndex, e);
		}


		#endregion


	}


}
