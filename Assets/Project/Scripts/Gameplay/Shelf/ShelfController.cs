using System;

namespace MarketTycoon.Gameplay.Shelf
{
    public class ShelfController : IDisposable
    {
        private readonly ShelfModel _model;
        private readonly ShelfView _view;

        public ShelfModel Model => _model;
        public ShelfView View => _view;

        public ShelfController (ShelfModel model, ShelfView view)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(_view));

            _model.OnStockChanged += _view.UpdateStockVisuals;
            _view.UpdateStockVisuals(_model.CurrentStock, _model.MaxCapacity);
        }

        public bool TryTakeProduct()
        {
            return _model.TryTakeItem();
        }

        public void Dispose()
        {
            if (_model != null)
            {
                _model.OnStockChanged -= _view.UpdateStockVisuals;
            }
        }
    }
}

