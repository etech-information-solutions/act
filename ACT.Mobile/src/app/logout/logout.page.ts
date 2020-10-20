import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { NavController } from '@ionic/angular';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.page.html',
  styleUrls: ['./logout.page.scss'],
})
export class LogoutPage implements OnInit 
{
  constructor( public auth: UserService, public navCtrl: NavController ) 
  {
  }

  ngOnInit() 
  {
    this.auth.LogOut();
    this.navCtrl.navigateRoot( "login" );
  }

}
