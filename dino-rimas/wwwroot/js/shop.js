document.addEventListener("DOMContentLoaded", () => {    
    showItem("shop-item");
    window.modalActon = "#";
})

function showItem(type) {
    for (const el of document.getElementsByClassName("shop-item")) {
        if (el.classList.contains(type)) el.classList.remove("shop-hidden");
        else el.classList.add("shop-hidden");
        
    }
}

function filterDino(search){
    if (search.value == "") showItem("shop-item")
    else{
        for (const el of document.getElementsByClassName("shop-item")) {
            const name = el.querySelector(".shop-item-name").innerHTML.toLowerCase();
            const s = search.value.toLowerCase()
            //console.log(name, s);
            if (name.indexOf(s) < 0)el.classList.add("shop-hidden");                  
            else  el.classList.remove("shop-hidden");
        }
    }
}

function copyIdToBufer(el){
    navigator.clipboard.writeText(el.innerHTML)
    .then(() => {
      alert("Id скопирован в буфер обмена.")
    })
    .catch(err => {
      console.log('Something went wrong', err);
    });
}

function hideModal() {
    const modal = document.getElementById("modal");
    modal.classList.remove("modal-show");
}
function goToAction(){
    window.location.href = window.modalAction;
}
function showModal(id, name, balance, price, isLogin) {
    const modal = document.getElementById("modal");
    if(isLogin == 'True'){
        if (balance < price) {
            window.modalAction = "/User/Donate";
            modal.querySelector('.modal-tittle').innerHTML = `Произошла ошибка`;
            modal.querySelector('.modal-body').innerHTML = `Вам не хватает ${price - balance} Dino Coin. Желаете пополнить?`;
            modal.querySelector('.modal-button-1').innerHTML = `Пополнить`;
            modal.classList.add("modal-show");
        } else {
            window.modalAction = "/DinoShop/BuyDino/" + id;
            modal.querySelector('.modal-tittle').innerHTML = `Покупка динозавра`;
            modal.querySelector('.modal-body').innerHTML = `Вы действительно хотите купить ${name} за ${price} Dino Coin?`;
            modal.querySelector('.modal-button-1').innerHTML = `Купить`;
            modal.classList.add("modal-show");
        }
    }else{
        window.modalAction = "/User/SignIn";
        modal.querySelector('.modal-tittle').innerHTML = `Внимание`;
        modal.querySelector('.modal-body').innerHTML = `Пожалуйста авторизируйтесь чере Steam для продожения.`;
        modal.querySelector('.modal-button-1').innerHTML = `Авторизироваться`;
        modal.classList.add("modal-show");
    }

    
}