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
	
	private NetworkVariable<State> state = new(State.Idle);

	private NetworkVariable<float> fryingTimer = new(0);
	private NetworkVariable<float> burningTimer = new(0);
	private FryingRecipeSO fryingRecipeSO;
	private BurningRecipeSO burningRecipeSO;

	public override void OnNetworkSpawn() {
		fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
		burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
		state.OnValueChanged += State_OnValueChanged;

	}

	private void FryingTimer_OnValueChanged(float previousValue, float newValue) {
		float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;
		OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
			progressNormalized = fryingTimer.Value / fryingTimerMax
		});
	}

	private void BurningTimer_OnValueChanged(float previousValue, float newValue) {
		float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.buringTimerMax : 1f;
		OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
			progressNormalized = burningTimer.Value / burningTimerMax
		});
	}

	private void State_OnValueChanged(State previousValue, State newValue) {
		OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state.Value });

		if (newValue == State.Idle || newValue == State.Burned) {
			OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
				progressNormalized = 0
			});
		}
	}

	private void Update() {
		if (!IsServer) return;

		if (HasKitchenObject()) {
			switch (state.Value) {
				case State.Idle: break;
				case State.Frying:
					fryingTimer.Value += Time.deltaTime;
					if (fryingTimer.Value >= fryingRecipeSO.fryingTimerMax) {
						KitchenObject.DestroyKitchenObject(GetKitchenObject());
						KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
						
						state.Value = State.Fried;
						burningTimer.Value = 0;

						SetBurningRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO()));
					}
					break;
				case State.Fried:
					burningTimer.Value += Time.deltaTime;
					if (burningTimer.Value >= burningRecipeSO.buringTimerMax) {
						KitchenObject.DestroyKitchenObject(GetKitchenObject());
						KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);
						
						state.Value = State.Burned;
						burningTimer.Value = 0;
					}
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
						KitchenObject.DestroyKitchenObject(GetKitchenObject());
						SetStateIdleServerRpc();
					}
				}
			} else {
				//Player is not carrying anything
				GetKitchenObject().SetKitchenObjectParent(player);
				SetStateIdleServerRpc();
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetStateIdleServerRpc() {
		state.Value = State.Idle;
	}


	[ServerRpc(RequireOwnership = false)]
	private void InteractLogicPlaceobjectOnCounterServerRpc(int kitchenObjectSOIndex) {
		fryingTimer.Value = 0;
		state.Value = State.Frying;
		SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
	}

	[ClientRpc]
	private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex) {
		KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectFromIndex(kitchenObjectSOIndex);
		fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
	}


	[ClientRpc]
	private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex) {
		KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectFromIndex(kitchenObjectSOIndex);
		burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
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
		return state.Value == State.Fried;
	}
}
