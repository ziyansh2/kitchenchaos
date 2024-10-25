using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress {
	public enum State {
		Idle,
		Frying,
		Fried,
		Burned
	}

	public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;


	public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
	public class OnStateChangedEventArgs : EventArgs {
		public State state;
	}

	[SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
	[SerializeField] private BurningRecipeSO[] burningRecipeSOArray;
	private State state;

	private NetworkVariable<float> fryingTimer = new(0);
	private float buringTimer;
	private FryingRecipeSO fryingRecipeSO;
	private BurningRecipeSO burningRecipeSO;

	private void Start() {
		state = State.Idle;
	}

	public override void OnNetworkSpawn() {
		fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;

	}

	private void FryingTimer_OnValueChanged(float previousValue, float newValue) {
		float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;
		OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
			progressNormalized = fryingTimer.Value / fryingTimerMax
		});
	}

	private void Update() {
		if (!IsServer) return;

		if (HasKitchenObject()) {
			switch (state) {
				case State.Idle: break;
				case State.Frying:
					fryingTimer.Value += Time.deltaTime;
					if (fryingTimer.Value >= fryingRecipeSO.fryingTimerMax) {
						GetKitchenObject().DestroySelf();
						KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

						UpdateState(State.Fried);
						buringTimer = 0;
						burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
					}
					
					break;
				case State.Fried:
					buringTimer += Time.deltaTime;
					if (buringTimer >= burningRecipeSO.buringTimerMax) {
						GetKitchenObject().DestroySelf();
						KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

						UpdateState(State.Burned);
						buringTimer = 0;
					}
					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { progressNormalized = buringTimer / burningRecipeSO.buringTimerMax });
					break;
				case State.Burned: break;
			}
		}
	}

	public override void Interact(Player player) {
		if (!HasKitchenObject()) {
			//There is no kitchen Object
			if (player.HasKitchenObject()) {
				//Player is carrying something
				if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
					//Drop it to the counter
					KitchenObject kitchenObject = player.GetKitchenObject();
					kitchenObject.SetKitchenObjectParent(this);
					InteractLogicPlaceobjectOnCounterServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
				}
			}
		} else {
			//There is a kitchen Object
			if (player.HasKitchenObject()) {
				//Player is carrying something
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
					//Player is holding a Plate
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
						GetKitchenObject().DestroySelf();
						UpdateState(State.Idle);
						OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { progressNormalized = 0 });
					}
				}
			} else {
				//Player is not carrying anything
				GetKitchenObject().SetKitchenObjectParent(player);
				UpdateState(State.Idle);
				OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { progressNormalized = 0 });
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void InteractLogicPlaceobjectOnCounterServerRpc(int kitchenObjectSOIndex) {
		InteractLogicPlaceobjectOnCounterClientRpc(kitchenObjectSOIndex);
	}

	[ClientRpc]
	private void InteractLogicPlaceobjectOnCounterClientRpc(int kitchenObjectSOIndex) {
		KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectFromIndex(kitchenObjectSOIndex);
		fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
		UpdateState(State.Frying);
		fryingTimer.Value = 0;
		OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { progressNormalized = fryingTimer.Value / fryingRecipeSO.fryingTimerMax });
	}

	private void UpdateState(State newState) {
		state = newState;
		OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
	}

	private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObejectSO) {
		return GetFryingRecipeSOWithInput(inputKitchenObejectSO) != null;
	}

	private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObejectSO) {
		return GetFryingRecipeSOWithInput(inputKitchenObejectSO)?.output;
	}
	private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObejectSO) {
		foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray) {
			if (fryingRecipeSO.input == inputKitchenObejectSO) {
				return fryingRecipeSO;
			}
		}
		return null;
	}

	private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObejectSO) {
		foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray) {
			if (burningRecipeSO.input == inputKitchenObejectSO) {
				return burningRecipeSO;
			}
		}
		return null;
	}

	public bool IsFried() {
		return state == State.Fried;
	}
}
