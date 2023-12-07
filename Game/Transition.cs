namespace GameProject
{
    public class Transition
    {
        private enum Direction { Left = -1, Right = 1}

        private const string ExtremitySpriteName = @"transition_crescent";

        private bool _isTransitioning;
        private float _timeElapsed;
        private float _length;
        private float _direction;
        private float _initialPosition;
        private bool _hasReachedHalfway;
        private Action _halfwayReached;
        private Action _endReached;

        public Transition(float length)
        {
            //SUGGESTION:
            // I would recommend a transition length between 0.25 and 0.75 seconds.
            // The idea: make it as fast as it can while still feeling like a microscope.

            _length = length;
        }

        public void Render()
        {
            if (!_isTransitioning) return;

            // position = _initialPosition + _direction * totalDistance * _time / _length

            //SUGGESTION:
            //Use the transition_crescent sprite on both sides of a blakc rectangle.
        }

        public void Update(float time)
        {
            if (!_isTransitioning) return;

            //NOTE: Due to potential lag and/or very short transition length, we have to make it possible
            //      for the halfway callback to be called on the same frame as the end callback.
            if (!_hasReachedHalfway && _timeElapsed >= _length * 0.5f)
            {
                _hasReachedHalfway = true;
                _halfwayReached?.Invoke();
            }

            if(_timeElapsed >= _length)
            {
                _isTransitioning = false;
                _endReached?.Invoke();
                return;
            }

            _timeElapsed += time;
        }

        public void Begin(Direction direction, Action onHalfwayReached = null, Action onEndReached = null)
        {
            //SUGGESTION:
            //When zooming in, use direction Left. When zooming out, use direnction Right.
            //When calling this method, block the zooming functionality.
            //On halfwat callback, register a method that applies the zoom in one shot.
            //On end callback, restore zooming functionality.

            //SUGGESTION++:
            //On halfway callback, without resuming zoom functionality, listen for zoom input
            //On end callback, if a zoom input was given after the halfway callback, apply that zoom

            if (_isTransitioning) return;

            _timeElapsed = 0;
            _halfwayReached = onHalfwayReached;
            _endReached = onEndReached;
            _direction = (float)direction;
            _hasReachedHalfway = false;

            //TODO: calculate initial position based on direction (if going left, start right, and vice versa).
            //_initialPosition = -_direction * someVisualSize.half.x; // assuming the transition visual origin is in the center
        }
    }
}